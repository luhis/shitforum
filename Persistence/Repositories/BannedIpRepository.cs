using System;
using System.Threading.Tasks;
using Domain;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

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
            return this.client.BannedImages.AnyAsync(a => a.Hash == hash.Val);
        }
    }
}
