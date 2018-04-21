using Microsoft.Extensions.DependencyInjection;

namespace ReCaptchaCore
{
    public static class DIModule
    {
        public static void Add(IServiceCollection services)
        {
            services.AddSingleton<IGetCaptchaValue, GetCaptchaValue>();
            services.AddSingleton<IRecaptchaVerifier, RecaptchaVerifier>();
            services.AddSingleton<RecaptchaSettings>();
        }
    }
}
