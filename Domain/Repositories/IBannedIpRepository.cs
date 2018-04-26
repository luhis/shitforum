using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IBannedIpRepository
    {
        Task<bool> IsBanned(IpHash.IIpHash hash);

        Task Ban(IpHash.IIpHash hash, string reason);
    }
}