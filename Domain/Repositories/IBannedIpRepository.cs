using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using Domain.IpHash;
using Optional;

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

        [Pure]
        Task<Option<DateTime>> GetByHash(IIpHash hash, CancellationToken cancellationToken);
    }
}
