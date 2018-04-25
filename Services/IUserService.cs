using System.Threading.Tasks;
using Domain;

namespace Services
{
    public interface IUserService
    {
        Task BanUser(IpHash hash, string reason);
    }
}