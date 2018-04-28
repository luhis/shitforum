using System;
using System.Threading.Tasks;
using Domain.IpHash;
using Optional;

namespace Services
{
    public interface IUserService
    {
        Task BanUser(IIpHash hash, string reason, DateTime expiry);

        Task<Option<IIpHash>> GetHashForPost(Guid postId);
    }
}
