using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace ReCaptchaCore
{
    public class RecaptchaVerifier : IRecaptchaVerifier
    {
        private readonly RecaptchaSettings settings;

        private static readonly string Api = "https://www.google.com/recaptcha/api/siteverify";

        public RecaptchaVerifier(IOptions<RecaptchaSettings> settings)
        {
            this.settings = settings.Value;
        }

        async Task<bool> IRecaptchaVerifier.IsValid(string response, IPAddress ip)
        {
            var client = new HttpClient();
            var result = await client.PostAsync(Api, new FormUrlEncodedContent(new Dictionary<string, string>() {
                { "secret", settings.PrivateKey },
                { "response", response },
                { "ipaddress", ip.ToString() },
            }));
            var s = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ReCaptchaResponse>(s).Success;
        }
    }
}
