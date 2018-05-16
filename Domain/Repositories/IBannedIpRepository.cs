using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.IpHash;

namespace Domain.Repositories
{
    public interface IBannedIpRepository
    {
        Task<bool> IsBanned(IpHash.IIpHash hash, CancellationToken cancellationToken);
        
        Task Ban(IIpHash hash, string reason, DateTime expiry);

        Task<IReadOnlyList<BannedIp>> GetAll(CancellationToken cancellationToken);
    }
}
