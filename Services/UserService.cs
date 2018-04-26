using System.Threading.Tasks;
using Domain;
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

        Task IUserService.BanUser(IIpHash hash, string reason)
        {
            return this.bannedIpRepository.Ban(hash, reason);
        }
    }
}