using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Optional;
using Services.Dtos;

namespace Services
{
    public class ThreadService : IThreadService
    {
        private readonly IThreadRepository threadRepository;
        private readonly IFileRepository fileRepository;
        private readonly IBoardRepository boardRepository;
        private readonly IPostRepository postRepository;

        public ThreadService(
            IThreadRepository threadRepository,
            IPostRepository postRepository,
            IFileRepository filesRepository,
            IBoardRepository boardRepository)
        {
            this.threadRepository = threadRepository;
            this.postRepository = postRepository;
            this.fileRepository = filesRepository;
            this.boardRepository = boardRepository;
        }

        private IQueryable<Guid> GetOrderedThreads(Guid boardId, Option<string> filter)
        {
            var boardThreads = this.threadRepository.GetAll().Where(t => t.BoardId == boardId).Select(t => t.Id);
            var threadIds = this.postRepository.GetAll().Where(a => boardThreads.Contains(a.ThreadId)).GroupBy(a => a.ThreadId).Where(a => a.OrderBy(b => b.Created).First().Comment.Contains(filter.ValueOr(string.Empty))).Select(a => a.Key);
            return this.postRepository.GetAll().Where(a => !a.IsSage && threadIds.Contains(a.ThreadId))
                    .OrderBy(a => a.Created).Select(a => a.ThreadId).Distinct();
        }

        async Task<Option<CatalogThreadOverViewSet>> IThreadService.GetOrderedCatalogThreads(string boardKey, CancellationToken cancellationToken)
        {
            var board = await this.boardRepository.GetByKey(boardKey, cancellationToken);
            return await board.Match(async some =>
            {
                var allThreads = GetOrderedThreads(some.Id, Option.None<string>());
                var domainThreads = await this.threadRepository.GetAll().Where(a => allThreads.Contains(a.Id)).ToListAsync(cancellationToken);
                var l = await Task.WhenAll(domainThreads.Select(async thread =>
                {
                    var posts = this.postRepository.GetAll().Where(p => p.ThreadId == thread.Id);
                    var firstPost = await this.GetFirstPostAsync(posts, cancellationToken);
                    return new CatalogThreadOverView(thread.Id, thread.Subject, some, firstPost);
                }).ToArray());
                return Option.Some(new CatalogThreadOverViewSet(some, l));
            }, () => Task.FromResult(Option.None<CatalogThreadOverViewSet>()));
        }

        async Task<Option<ThreadOverViewSet>> IThreadService.GetOrderedThreads(string boardKey, Option<string> filter, int pageSize, int pageNumber, CancellationToken cancellationToken)
        {
            var board = await this.boardRepository.GetByKey(boardKey, cancellationToken);
            return await board.Match(async some =>
            {
                var threadIds = GetOrderedThreads(some.Id, filter);
                var latestThreads = threadIds.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
                var threads = await this.threadRepository.GetAll().Where(a => latestThreads.Contains(a.Id)).ToListAsync(cancellationToken);
                var l = await Task.WhenAll(threads.Select(async thread =>
                {
                    var posts = this.postRepository.GetAll().Where(p => p.ThreadId == thread.Id);
                    var firstPost = await GetFirstPostAsync(posts, cancellationToken);
                    var lastPosts = (await Task.WhenAll(posts.Skip(1).OrderByDescending(a => a.Created).Take(5).ToArray().Select(async p =>
                    {
                        var file = await this.fileRepository.GetPostFile(p.Id, cancellationToken);
                        return PostMapper.Map(p, file);
                    }))).OrderBy(a => a.Created).ToList();
                    var shownPosts = lastPosts.Concat(new[] { firstPost }).ToList();
                    var stats = await GetOveriewStats(thread.Id, posts, shownPosts, cancellationToken);
                    return new ThreadOverView(thread.Id, thread.Subject, firstPost, lastPosts, stats);
                }).ToArray());
                var numberOfPages = (threadIds.Count() / pageSize) + 1;
                return Option.Some(new ThreadOverViewSet(some, l, new PageData(pageNumber, numberOfPages)));
            }, () => Task.FromResult(Option.None<ThreadOverViewSet>()));
        }

        private async Task<PostOverView> GetFirstPostAsync(IQueryable<Domain.Post> posts, CancellationToken cancellationToken)
        {
            var firstPost = await posts.OrderBy(a => a.Created).FirstAsync(cancellationToken);
            var file = await this.fileRepository.GetPostFile(firstPost.Id, cancellationToken);
            return PostMapper.Map(firstPost, file);
        }

        async Task<Option<ThreadDetailView>> IThreadService.GetThread(Guid threadId, int pageSize, CancellationToken cancellationToken)
        {
            Task<Option<ThreadDetailView>> None() => Task.FromResult(Option.None<ThreadDetailView>());

            var thread = await this.threadRepository.GetById(threadId, cancellationToken);
            return await thread.Match(async t =>
            {
                var posts = this.postRepository.GetAll().Where(a => a.ThreadId == threadId);
                var b = await this.boardRepository.GetById(t.BoardId, cancellationToken);
                return await b.Match(async some =>
                    {
                        var postsMapped = await Task.WhenAll(posts
                            .OrderBy(a => a.Created).ToList()
                            .Select(async p => PostMapper.Map(p, await this.fileRepository.GetPostFile(p.Id, cancellationToken))));
                        var stats = await GetStats(postsMapped, some.Id, t.Id, pageSize);
                        return Option.Some(new ThreadDetailView(threadId, t.Subject, stats,
                            new BoardOverView(some.Id, some.BoardName, some.BoardKey), postsMapped.ToList()));
                    },
                    None);
            }, None);
        }

        private async Task<int> GetThreadPageNumber(Guid boardId, Guid threadId, int pageSize)
        {
            var ids = await this.GetOrderedThreads(boardId, Option.None<string>()).ToListAsync();
            return  (ids.FindIndex(a => a == threadId) / pageSize) + 1;
        }

        private async Task<ThreadStats> GetStats(IReadOnlyList<PostOverView> posts, Guid boardId, Guid threadId, int pageSize)
        {
            var replies = posts.Count - 1;
            var images = posts.Count(a => a.File.HasValue) - 1;
            var posters = posts.Select(p => p.IpHash).Distinct().Count();
            var page = await GetThreadPageNumber(boardId, threadId, pageSize);
            return new ThreadStats(replies, images, posters, page);
        }

        private async Task<ThreadOverViewStats> GetOveriewStats(Guid threadId, IQueryable<Domain.Post> posts, IReadOnlyList<PostOverView> shownPosts, CancellationToken cancellationToken)
        {
            var postCount = posts.Count() - shownPosts.Count;
            var imageCount = await this.fileRepository.GetImageCount(threadId, cancellationToken) - shownPosts.Count(p => p.File.HasValue);
            return new ThreadOverViewStats(postCount, imageCount);
        }
    }
}
