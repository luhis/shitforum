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
                return new CatalogThreadOverView(threadId, thread.Subject, board.ValueOr((Board)null), firstPost);
            }).ToArray());
            return board.Match(some => Option.Some(new CatalogThreadOverViewSet(some, threads)), () => Option.None<CatalogThreadOverViewSet>());
        }

        async Task<Option<ThreadOverViewSet>> IThreadService.GetOrderedThreads(Guid boardId, int pageSize, int pageNumber)
        {
            var latestThreads = this.postsRepository.GetAll().Where(a => !a.IsSage).OrderBy(a => a.Created).Select(a => a.ThreadId).Distinct()
                .Skip(pageSize * pageNumber).Take(pageSize).ToArray();
            var board = await this.boardRepository.GetById(boardId);
            var l = await Task.WhenAll(latestThreads.Select(async threadId =>
            {
                var thread = await this.threadsRepository.GetById(threadId);
                var posts = await this.postsRepository.GetAll(threadId);
                var firstPost = await GetFirstPostAsync(posts);
                var lastPosts = await Task.WhenAll(posts.OrderByDescending(a => a.Created).Take(5).Select(async p =>
                {
                    var file = await this.fileRepository.GetPostFile(p.Id);
                    return PostMapper.Map(p, file);
                }));
                var postCount = posts.Count();
                var imageCount = await this.fileRepository.GetImageCount(threadId);
                return new ThreadOverView(threadId, thread.Subject,  firstPost, lastPosts.ToList(), postCount, imageCount);
            }).ToArray());
            return board.Match(some => Option.Some(new ThreadOverViewSet(some, l)), () => Option.None<ThreadOverViewSet>());
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

            var b = await this.boardRepository.GetById(thread.BoardId);
            return b.Match(some => Option.Some(new ThreadDetailView(threadId, thread.Subject, some, postsMapped.ToList())), () => Option.None<ThreadDetailView>());
        }
    }
}
