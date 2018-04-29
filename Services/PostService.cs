using Domain;
using Domain.Repositories;
using Optional;
using System;
using System.Threading.Tasks;
using Domain.IpHash;
using Services.Dtos;
using OneOf;
using Services.Results;

namespace Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository postRepository;
        private readonly IFileRepository fileRepository;
        private readonly IThreadRepository threadRepository;
        private readonly IBannedIpRepository bannedIpRepository;
        private readonly IBoardRepository boardRepository;

        public PostService(IPostRepository postRepository, IFileRepository fileRepository, IThreadRepository threadRepository, IBannedIpRepository bannedIpRepository, IBoardRepository boardRepository)
        {
            this.postRepository = postRepository;
            this.fileRepository = fileRepository;
            this.threadRepository = threadRepository;
            this.bannedIpRepository = bannedIpRepository;
            this.boardRepository = boardRepository;
        }

        private const int ImageLimit = 50;
        private const int PostLimit = 50;

        private Task<int> GetImageCount(Guid threadId) => this.fileRepository.GetImageCount(threadId);
        private Task<int> GetPostCountAsync(Guid threadId) => this.postRepository.GetThreadPostCount(threadId);

        async Task<OneOf<Success, Banned, ImageCountExceeded, PostCountExceeded>> IPostService.Add(Guid postId, Guid threadId, TripCodedName name, string comment, bool isSage, IIpHash ipAddress, Option<File> file)
        {
            if (await this.bannedIpRepository.IsBanned(ipAddress))
            {
                return new Banned();
            }

            if (ImageLimit <= await this.GetImageCount(threadId))
            {
                return new ImageCountExceeded();
            }

            if (PostLimit <= await this.GetPostCountAsync(threadId))
            {
                return new PostCountExceeded();
            }

            var post = new Domain.Post(postId, threadId, DateTime.UtcNow, name.Val, comment, isSage, ipAddress.Val);
            await this.postRepository.Add(post).ConfigureAwait(false);
            await file.Match(some => this.fileRepository.Add(some), () => Task.CompletedTask);

            return new Success();
        }

        async Task<OneOf<Success, Banned>> IPostService.AddThread(Guid postId, Guid threadId, Guid boardId, string subject, TripCodedName name, string comment, bool isSage, IIpHash ipAddress, Option<File> file)
        { 
            if (await this.bannedIpRepository.IsBanned(ipAddress))
            {
                return new Banned();
            }

            var thread = new Thread(threadId, boardId, subject);
            await threadRepository.Add(thread).ConfigureAwait(false);
            var post = new Domain.Post(postId, threadId, DateTime.UtcNow, name.Val, comment, isSage, ipAddress.Val);
            await this.postRepository.Add(post).ConfigureAwait(false);
            await file.Match(some => this.fileRepository.Add(some), () => Task.CompletedTask);

            return new Success();
        }

        async Task<Option<PostContextView>> IPostService.GetById(Guid id)
        {
            Func<Task<Option<PostContextView>>> noneRes = () => Task.FromResult(Option.None<PostContextView>());
            var post = await this.postRepository.GetById(id);

            return await post.Match(async some =>
            {
                var t = await this.threadRepository.GetById(some.ThreadId);
                return await t.Match(async thread =>
                {
                    var b = await this.boardRepository.GetById(thread.BoardId);
                    var file = await this.fileRepository.GetPostFile(some.Id);
                    return b.Match(
                        board => 
                        Option.Some(new PostContextView(thread.Id, thread.Subject, new BoardOverView(board.Id, board.BoardName, board.BoardKey), PostMapper.Map(some, file))),
                        Option.None<PostContextView>);
                }, noneRes);
            }, noneRes);
        }

        async Task<bool> IPostService.DeletePost(Guid id)
        {
            var p = await this.postRepository.GetById(id);
            return await p.Match(async post =>
            {
                var postCount = await this.postRepository.GetThreadPostCount(post.ThreadId);
                await this.postRepository.Delete(post);
                if (postCount == 1)
                {
                    var thread = await this.threadRepository.GetById(post.ThreadId);
                    await thread.Match(some => this.threadRepository.Delete(some), () => Task.CompletedTask);
                }
                return true;
            }, () => Task.FromResult(false));
        }
    }
}
