using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.IpHash;
using Domain.Repositories;
using Optional;
using Services.Interfaces;

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

        async Task<Option<IIpHash>> IUserService.GetHashForPost(Guid postId, CancellationToken cancellationToken)
        {
            var post = await this.postRepository.GetById(postId, cancellationToken);
                
            return post.Map(some => new IpHash(some.IpAddress) as IIpHash);
        }

        Task<IReadOnlyList<BannedIp>> IUserService.GetAllBans(CancellationToken cancellationToken)
        {
            return this.bannedIpRepository.GetAll(cancellationToken);
        }
    }
}
