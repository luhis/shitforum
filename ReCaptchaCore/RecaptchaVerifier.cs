using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ReCaptchaCore
{
    public class RecaptchaVerifier : IRecaptchaVerifier
    {
        private readonly RecaptchaSettings settings;

        private readonly static string Api = "https://www.google.com/recaptcha/api/siteverify";

        public RecaptchaVerifier(RecaptchaSettings settings)
        {
            this.settings = settings;
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
