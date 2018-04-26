using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Repositories;
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
            var latestThreads = this.postsRepository.GetAll().Where(a => !a.IsSage).OrderBy(a => a.Created).Select(a => a.ThreadId).Distinct()
                .Skip(pageSize * pageNumber).Take(pageSize).ToArray();
            var board = await this.boardRepository.GetById(boardId);
            var threads = await Task.WhenAll(latestThreads.Select(async threadId =>
            {
                var thread = await this.threadsRepository.GetById(threadId);
                var posts = await this.postsRepository.GetAll(threadId);
                var firstPost = await this.GetFirstPostAsync(posts);
                return new CatalogThreadOverView(threadId, thread.ValueOr((Thread)null).Subject, board.ValueOr((Board)null), firstPost);
            }).ToArray());
            return board.Match(some => Option.Some(new CatalogThreadOverViewSet(some, threads)), Option.None<CatalogThreadOverViewSet>);
        }

        async Task<Option<ThreadOverViewSet>> IThreadService.GetOrderedThreads(Guid boardId, Option<string> filter, int pageSize, int pageNumber)
        {
            var threadIds = this.threadsRepository.GetAll().Where(a => a.Posts.OrderBy(p => p.Created).First().Comment.Contains(filter.ValueOr(string.Empty))).Select(t => t.Id);
            var latestThreads = this.postsRepository.GetAll().Where(a => !a.IsSage && threadIds.Contains(a.ThreadId)).OrderBy(a => a.Created).Select(a => a.ThreadId).Distinct()
                .Skip(pageSize * pageNumber).Take(pageSize).ToArray();
            var board = await this.boardRepository.GetById(boardId);
            var l = await Task.WhenAll(latestThreads.Select(async threadId =>
            {
                var thread = await this.threadsRepository.GetById(threadId);
                var posts = await this.postsRepository.GetAll(threadId);
                var firstPost = await GetFirstPostAsync(posts);
                var lastPosts = (await Task.WhenAll(posts.Skip(1).OrderByDescending(a => a.Created).Take(5).Select(async p =>
                {
                    var file = await this.fileRepository.GetPostFile(p.Id);
                    return PostMapper.Map(p, file);
                }))).OrderBy(a => a.Created).ToList();
                var shownPosts = lastPosts.Concat(new[] { firstPost });
                var postCount = posts.Count() - (shownPosts.Count());
                var imageCount = (await this.fileRepository.GetImageCount(threadId)) - shownPosts.Count(p => p.File.HasValue);
                return new ThreadOverView(threadId, thread.ValueOr((Thread)null).Subject, firstPost, lastPosts, postCount, imageCount);
            }).ToArray());
            return board.Match(some => Option.Some(new ThreadOverViewSet(some, l)), Option.None<ThreadOverViewSet>);
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
