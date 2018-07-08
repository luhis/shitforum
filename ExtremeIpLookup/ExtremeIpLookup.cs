using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExtremeIpLookup
{
    public class ExtremeIpLookup : IExtremeIpLookup
    {
        private static string GetUrl(IPAddress ip) => $"https://extreme-ip-lookup.com/json/{ip}";
        private static readonly HttpClient client = new HttpClient();

        async Task<ResultObject> IExtremeIpLookup.GetIpDetailsAsync(IPAddress ip)
        {
            var json = await client.GetStringAsync(GetUrl(ip));
            return Deserialise(json);
        }

        public static ResultObject Deserialise(string json) => JsonConvert.DeserializeObject<ResultObject>(json);
    }
}
