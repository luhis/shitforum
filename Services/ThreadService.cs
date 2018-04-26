using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Optional;
using Services.Dtos;

namespace Services
{
    public class ThreadService : IThreadService
    {
        private readonly IThreadRepository threadsRepository;
        private readonly IPostRepository postsRepository;
        private readonly IFileRepository fileRepository;
        private readonly IBoardRepository boardRepository;

        public ThreadService(IThreadRepository threadsRepository, IPostRepository postsRepository, IFileRepository filesRepository, IBoardRepository boardRepository)
        {
            this.threadsRepository = threadsRepository;
            this.postsRepository = postsRepository;
            this.fileRepository = filesRepository;
            this.boardRepository = boardRepository;
        }

        async Task<Option<CatalogThreadOverViewSet>> IThreadService.GetOrderedCatalogThreads(Guid boardId, int pageSize, int pageNumber)
        {
            var threadIds = this.threadsRepository.GetAll().Where(t => t.BoardId == boardId).Select(t => t.Id);
            var latestThreads = this.postsRepository.GetAll().Where(a => !a.IsSage && threadIds.Contains(a.ThreadId)).OrderBy(a => a.Created).Select(a => a.ThreadId).Distinct()
                .Skip(pageSize * pageNumber).Take(pageSize);
            var threads = await this.threadsRepository.GetAll().Where(a => latestThreads.Contains(a.Id)).ToListAsync();
            var board = await this.boardRepository.GetById(boardId);
            return await board.Match(async some =>
            {
                var t = await Task.WhenAll(threads.Select(async thread =>
                {
                    var posts = await this.postsRepository.GetAll(thread.Id);
                    var firstPost = await this.GetFirstPostAsync(posts);
                    return new CatalogThreadOverView(thread.Id, thread.Subject, some, firstPost);
                }).ToArray());
                return Option.Some(new CatalogThreadOverViewSet(some, t));
            }, () => Task.FromResult(Option.None<CatalogThreadOverViewSet>()));
        }

        async Task<Option<ThreadOverViewSet>> IThreadService.GetOrderedThreads(Guid boardId, Option<string> filter, int pageSize, int pageNumber)
        {
            var threadIds = this.threadsRepository.GetAll().Where(t => t.BoardId == boardId).Where(a => a.Posts.OrderBy(p => p.Created).First().Comment.Contains(filter.ValueOr(string.Empty))).Select(t => t.Id);
            var latestThreads = this.postsRepository.GetAll().Where(a => !a.IsSage && threadIds.Contains(a.ThreadId)).OrderBy(a => a.Created).Select(a => a.ThreadId).Distinct()
                .Skip(pageSize * pageNumber).Take(pageSize);
            var threads = await this.threadsRepository.GetAll().Where(a => latestThreads.Contains(a.Id)).ToListAsync();
            var board = await this.boardRepository.GetById(boardId);
            return await board.Match(async some =>
            {
                var l = await Task.WhenAll(threads.Select(async thread =>
                {
                    var posts = await this.postsRepository.GetAll(thread.Id);
                    var firstPost = await GetFirstPostAsync(posts);
                    var lastPosts = (await Task.WhenAll(posts.Skip(1).OrderByDescending(a => a.Created).Take(5).Select(async p =>
                    {
                        var file = await this.fileRepository.GetPostFile(p.Id);
                        return PostMapper.Map(p, file);
                    }))).OrderBy(a => a.Created).ToList();
                    var shownPosts = lastPosts.Concat(new[] { firstPost });
                    var postCount = posts.Count() - (shownPosts.Count());
                    var imageCount = (await this.fileRepository.GetImageCount(thread.Id)) - shownPosts.Count(p => p.File.HasValue);
                    return new ThreadOverView(thread.Id, thread.Subject, firstPost, lastPosts, postCount, imageCount);
                }).ToArray());
                return Option.Some(new ThreadOverViewSet(some, l));
            }, () => Task.FromResult(Option.None<ThreadOverViewSet>()));
        }

        private async Task<PostOverView> GetFirstPostAsync(IEnumerable<Domain.Post> posts)
        {
            var firstPost = posts.OrderBy(a => a.Created).First();
            var file = await this.fileRepository.GetPostFile(firstPost.Id);
            return PostMapper.Map(firstPost, file);
        }

        async Task<Option<ThreadDetailView>> IThreadService.GetThread(Guid threadId)
        {
            var thread = await this.threadsRepository.GetById(threadId);
            var posts = await this.postsRepository.GetAll(threadId);
            var postsMapped = await Task.WhenAll(posts.OrderBy(a => a.Created).Select(async p =>
            {
                var file = await this.fileRepository.GetPostFile(p.Id);
                return PostMapper.Map(p, file);
            }));
            return await thread.Match(async t =>
            {
                var b = await this.boardRepository.GetById(t.BoardId);
                return b.Match(
                    some => Option.Some(new ThreadDetailView(threadId, t.Subject, some, postsMapped.ToList())), 
                    Option.None<ThreadDetailView>);

            }, () => Task.FromResult(Option.None<ThreadDetailView>()));
        }
    }
}
