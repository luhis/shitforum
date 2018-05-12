using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.IpHash;
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

        Task IBannedIpRepository.Ban(IIpHash hash, string reason, DateTime expiry)
        {
            this.client.BannedIp.Add(new BannedIp(Guid.NewGuid(), hash.Val, reason, expiry));
            return this.client.SaveChangesAsync();
        }

        Task<IReadOnlyList<BannedIp>> IBannedIpRepository.GetAll()
        {
            return this.client.BannedIp.ToReadOnlyAsync();
        }

        Task<bool> IBannedIpRepository.IsBanned(IIpHash hash)
        {
            return this.client.BannedIp.AnyAsync(a => a.IpHash == hash.Val);
        }
    }
}
