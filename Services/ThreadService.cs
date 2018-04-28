using System;
using System.Linq;
using System.Threading.Tasks;
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

        async Task<Option<CatalogThreadOverViewSet>> IThreadService.GetOrderedCatalogThreads(string boardKey, int pageSize, int pageNumber)
        {
            var board = await this.boardRepository.GetByKey(boardKey);
            return await board.Match(async some =>
            {
                var threadIds = this.threadsRepository.GetAll().Where(thread => thread.BoardId == some.Id).Select(thread => thread.Id);
                var latestThreads = this.postsRepository.GetAll().Where(a => !a.IsSage && threadIds.Contains(a.ThreadId)).OrderBy(a => a.Created).Select(a => a.ThreadId).Distinct()
                    .Skip(pageSize * pageNumber).Take(pageSize);
                var threads = await this.threadsRepository.GetAll().Where(a => latestThreads.Contains(a.Id)).ToListAsync();
                var t = await Task.WhenAll(threads.Select(async thread =>
                {
                    var posts = this.postsRepository.GetAll().Where(p => p.ThreadId == thread.Id);
                    var firstPost = await this.GetFirstPostAsync(posts);
                    return new CatalogThreadOverView(thread.Id, thread.Subject, some, firstPost);
                }).ToArray());
                return Option.Some(new CatalogThreadOverViewSet(some, t));
            }, () => Task.FromResult(Option.None<CatalogThreadOverViewSet>()));
        }

        async Task<Option<ThreadOverViewSet>> IThreadService.GetOrderedThreads(string boardKey, Option<string> filter, int pageSize, int pageNumber)
        {
            var board = await this.boardRepository.GetByKey(boardKey);
            return await board.Match(async some =>
            {
                var threadIds = this.threadsRepository.GetAll().Where(t => t.BoardId == some.Id).Where(a => a.Posts.OrderBy(p => p.Created).First().Comment.Contains(filter.ValueOr(string.Empty))).Select(t => t.Id);
                var latestThreads = this.postsRepository.GetAll().Where(a => !a.IsSage && threadIds.Contains(a.ThreadId)).OrderBy(a => a.Created).Select(a => a.ThreadId).Distinct()
                    .Skip(pageSize * pageNumber).Take(pageSize);
                var threads = await this.threadsRepository.GetAll().Where(a => latestThreads.Contains(a.Id)).ToListAsync();
                var l = await Task.WhenAll(threads.Select(async thread =>
                {
                    var posts = this.postsRepository.GetAll().Where(p => p.ThreadId == thread.Id);
                    var firstPost = await GetFirstPostAsync(posts);
                    var lastPosts = (await Task.WhenAll(posts.Skip(1).OrderByDescending(a => a.Created).Take(5).ToArray().Select(async p =>
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

        private async Task<PostOverView> GetFirstPostAsync(IQueryable<Domain.Post> posts)
        {
            var firstPost = await posts.OrderBy(a => a.Created).FirstAsync();
            var file = await this.fileRepository.GetPostFile(firstPost.Id);
            return PostMapper.Map(firstPost, file);
        }

        async Task<Option<ThreadDetailView>> IThreadService.GetThread(Guid threadId)
        {
            var thread = await this.threadsRepository.GetById(threadId);
            return await thread.Match(async t =>
            {
                var posts = this.postsRepository.GetAll().Where(a => a.ThreadId == threadId);
                var postsMapped = await Task.WhenAll( posts
                    .OrderBy(a => a.Created).ToList()
                    .Select(async p => PostMapper.Map(p, await this.fileRepository.GetPostFile(p.Id))));
                var b = await this.boardRepository.GetById(t.BoardId);
                return b.Match(
                    some => Option.Some(new ThreadDetailView(threadId, t.Subject, new BoardOverView(some.Id, some.BoardName, some.BoardKey), postsMapped.ToList())), 
                    Option.None<ThreadDetailView>);

            }, () => Task.FromResult(Option.None<ThreadDetailView>()));
        }
    }
}
