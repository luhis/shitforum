using System.Threading.Tasks;
using Domain.IpHash;

namespace Services
{
    public interface IUserService
    {
        Task BanUser(IIpHash hash, string reason);
    }
}