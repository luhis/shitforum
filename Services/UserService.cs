using System;
using System.Threading.Tasks;
using Domain.IpHash;
using Domain.Repositories;
using Optional;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IBannedIpRepository bannedIpRepository;
        private readonly IPostRepository postRepository;

        public UserService(IBannedIpRepository bannedIpRepository, IPostRepository postRepository)
        {
            this.bannedIpRepository = bannedIpRepository;
            this.postRepository = postRepository;
        }

        Task IUserService.BanUser(IIpHash hash, string reason, DateTime expiry)
        {
            return this.bannedIpRepository.Ban(hash, reason, expiry);
        }

        public Task<bool> IsBanned(IIpHash hash)
        {
            return this.bannedIpRepository.IsBanned(hash);
        }

        public async Task<Option<IIpHash>> GetHashForPost(Guid postId)
        {
            var post = await this.postRepository.GetById(postId);
                
            return post.Match(some => Option.Some<IIpHash>(new IpHash(some.IpAddress)), Option.None<IIpHash>);
        }
    }
}
