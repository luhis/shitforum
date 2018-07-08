using System.Net;
using System.Threading.Tasks;

namespace ExtremeIpLookup
{
    public interface IExtremeIpLookup
    {
        Task<ResultObject> GetIpDetailsAsync(IPAddress ip);
    }
}
