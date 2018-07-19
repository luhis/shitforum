using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.IpHash;
using Optional;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task BanUser(IIpHash hash, string reason, DateTime expiry);

        Task<Option<IIpHash>> GetHashForPost(Guid postId, CancellationToken cancellationToken);

        Task<IReadOnlyList<BannedIp>> GetAllBans(CancellationToken cancellationToken);

        Task<Option<DateTime>> GetExpiry(IIpHash hash, CancellationToken cancellationToken);
    }
}
