using System;
using System.Threading.Tasks;
using Domain.IpHash;
using Domain.Repositories;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IBannedIpRepository bannedIpRepository;

        public UserService(IBannedIpRepository bannedIpRepository)
        {
            this.bannedIpRepository = bannedIpRepository;
        }

        Task IUserService.BanUser(IIpHash hash, string reason, DateTime expiry)
        {
            return this.bannedIpRepository.Ban(hash, reason, expiry);
        }

        public Task<bool> IsBanned(IIpHash hash)
        {
            return this.bannedIpRepository.IsBanned(hash);
        }
    }
}