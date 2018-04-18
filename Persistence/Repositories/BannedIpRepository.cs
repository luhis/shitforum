using System;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Repositories;

namespace Persistence.Repositories
{
    public class BannedIpRepository : IBannedIpRepository
    {
        private readonly ForumContext client;

        public BannedIpRepository(ForumContext client)
        {
            this.client = client;
        }

        Task IBannedIpRepository.Ban(IpHash hash, string reason)
        {
            this.client.BannedIp.Add(new BannedIp(Guid.NewGuid(), hash.Val, reason));
            return this.client.SaveChangesAsync();
        }

        Task<bool> IBannedIpRepository.IsBanned(IpHash hash)
        {
            return Task.FromResult(this.client.BannedImages.Where(a => a.Hash == hash.Val).Any());
        }
    }
}
