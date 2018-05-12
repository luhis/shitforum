using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
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

        async Task<Option<IIpHash>> IUserService.GetHashForPost(Guid postId)
        {
            var post = await this.postRepository.GetById(postId);
                
            return post.Match(some => Option.Some<IIpHash>(new IpHash(some.IpAddress)), Option.None<IIpHash>);
        }

        Task<IReadOnlyList<BannedIp>> IUserService.GetAllBans()
        {
            return this.bannedIpRepository.GetAll();
        }
    }
}
