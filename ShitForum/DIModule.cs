using Microsoft.Extensions.DependencyInjection;
using ShitForum.BannedImageLogger;
using ShitForum.Cookies;
using ShitForum.GetIp;
using ShitForum.Hasher;
using ShitForum.ImageValidation;
using Persistence;
using ShitForum.SettingsObjects;

namespace ShitForum
{
    public static class DIModule
    {
        public static void Add(IServiceCollection services)
        {
            services.AddSingleton<IValidateImage, ValidateImage>();
            services.AddSingleton<IShitForumDbConfig, ShitForumDbConfig>();
            services.AddSingleton<ICookieStorage, CookieStorage>();
            services.AddSingleton<IIsAdmin, IsAdmin>();
            services.AddSingleton<IBannedImageLogger, BannedImageLogger.BannedImageLogger>();
            services.AddSingleton<IGetIp, GetIp.GetIp>();
            services.AddSingleton<IpHasherFactory>();
            services.AddSingleton<TripCodeHasher>();
            services.AddSingleton<AdminSettings>();
            services.AddSingleton<ForumSettings>();
        }
    }
}
