using Domain;
using Domain.Repositories;
using Optional;
using System;
using System.Threading.Tasks;
using Services.Dtos;
using OneOf;
using Services.Results;
using System.Linq;

namespace Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository postRepository;
        private readonly IFileRepository fileRepository;
        private readonly IThreadRepository threadRepository;
        private readonly IBannedIpRepository bannedIpRepository;

        public PostService(IPostRepository postRepository, IFileRepository fileRepository, IThreadRepository threadRepository, IBannedIpRepository bannedIpRepository)
        {
            this.postRepository = postRepository;
            this.fileRepository = fileRepository;
            this.threadRepository = threadRepository;
            this.bannedIpRepository = bannedIpRepository;
        }

        private const int ImageLimit = 50;
        private const int PostLimit = 50;

        private Task<int> GetImageCount(Guid threadId) => this.fileRepository.GetImageCount(threadId);
        private Task<int> GetPostCountAsync(Guid threadId) => this.postRepository.GetThreadPostCount(threadId);

        async Task<OneOf<Success, Banned, ImageCountExceeded, PostCountExceeded>> IPostService.Add(Guid postId, Guid threadId, TripCodedName name, string comment, bool isSage, IpHash ipAddress, Option<File> file)
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

        async Task<OneOf<Success, Banned>> IPostService.AddThread(Guid postId, Guid threadId, Guid boardId, string subject, TripCodedName name, string comment, bool isSage, IpHash ipAddress, Option<File> file)
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

        Task<Option<Post>> IPostService.GetById(Guid id)
        {
            return this.postRepository.GetById(id);
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