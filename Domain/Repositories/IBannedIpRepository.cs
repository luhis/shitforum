using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using Domain.IpHash;

namespace Domain.Repositories
{
    public interface IBannedIpRepository
    {
        [Pure]
        Task<bool> IsBanned(IIpHash hash, CancellationToken cancellationToken);

        [Pure]
        Task Ban(IIpHash hash, string reason, DateTime expiry);

        [Pure]
        Task<IReadOnlyList<BannedIp>> GetAll(CancellationToken cancellationToken);
    }
}
