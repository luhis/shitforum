using EnsureThat;
using Microsoft.Extensions.Configuration;

namespace ReCaptchaCore
{
    public class RecaptchaSettings
    {
        public RecaptchaSettings(IConfiguration config)
        {
            this.PrivateKey = EnsureArg.IsNotNullOrWhiteSpace(config.GetSection("Recaptcha:PrivateKey").Get<string>());
            this.PublicKey = EnsureArg.IsNotNullOrWhiteSpace(config.GetSection("Recaptcha:PublicKey").Get<string>());
        }

        public string PrivateKey { get; }
        public string PublicKey { get; }
    }
}
