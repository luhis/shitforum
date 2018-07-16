using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.IpHash;
using Domain.Repositories;
using OneOf;
using Optional;
using Services.Dtos;
using Services.Interfaces;
using Services.Mappers;
using Services.Results;
using Thread = Domain.Thread;

namespace Services.Services
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

        async Task<OneOf<Success, Banned, ImageCountExceeded, PostCountExceeded>> IPostService.Add(Guid postId, Guid threadId, TripCodedName name, string comment, bool isSage, IIpHash ipAddress, Option<File> file, CancellationToken cancellationToken)
        {
            if (await this.bannedIpRepository.IsBanned(ipAddress, cancellationToken))
            {
                return new Banned();
            }

            if (ImageLimit <= await this.fileRepository.GetImageCount(threadId, cancellationToken))
            {
                return new ImageCountExceeded();
            }

            if (PostLimit <= await this.postRepository.GetThreadPostCount(threadId, cancellationToken))
            {
                return new PostCountExceeded();
            }

            var post = new Domain.Post(postId, threadId, DateTime.UtcNow, name.Val, comment, isSage, ipAddress.Val);
            await this.postRepository.Add(post).ConfigureAwait(false);
            await file.Match(some => this.fileRepository.Add(some), () => Task.CompletedTask);

            return new Success();
        }

        async Task<OneOf<Success, Banned>> IPostService.AddThread(Guid postId, Guid threadId, Guid boardId, string subject, TripCodedName name, string comment, bool isSage, IIpHash ipAddress, Option<File> file, CancellationToken cancellationToken)
        {
            if (await this.bannedIpRepository.IsBanned(ipAddress, cancellationToken))
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

        async Task<Option<PostContextView>> IPostService.GetById(Guid id, CancellationToken cancellationToken)
        {
            var post = await this.postRepository.GetById(id, cancellationToken);

            return (await post.MapToTask(async some =>
            {
                var t = await this.threadRepository.GetById(some.ThreadId, cancellationToken);
                return await t.MapToTask(async thread =>
                {
                    var b = await this.boardRepository.GetById(thread.BoardId, cancellationToken);
                    var file = await this.fileRepository.GetPostFile(some.Id, cancellationToken);
                    return b.Map(
                        board => 
                        new PostContextView(thread.Id, thread.Subject, new BoardOverView(board.Id, board.BoardName, board.BoardKey), PostMapper.Map(some, file)));
                });
            })).Flatten().Flatten();
        }

        async Task<bool> IPostService.DeletePost(Guid id, CancellationToken cancellationToken)
        {
            var p = await this.postRepository.GetById(id, cancellationToken);
            return await p.MapToTask(async post =>
            {
                var postCount = await this.postRepository.GetThreadPostCount(post.ThreadId, cancellationToken);
                await this.postRepository.Delete(post);
                if (postCount == 1)
                {
                    var thread = await this.threadRepository.GetById(post.ThreadId, cancellationToken);
                    await thread.MapToTask(some => this.threadRepository.Delete(some));
                }

                return true;
            }, false);
        }
    }
}
