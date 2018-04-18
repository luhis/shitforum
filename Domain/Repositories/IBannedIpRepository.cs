using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IBannedIpRepository
    {
        Task<bool> IsBanned(IpHash hash);

        Task Ban(IpHash hash, string reason);
    }
}