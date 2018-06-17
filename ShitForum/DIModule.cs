using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShitForum.BannedImageLogger;
using ShitForum.Cookies;
using ShitForum.GetIp;
using Hashers;
using ImageValidation;
using Persistence;
using ShitForum.IsAdmin;
using ShitForum.SettingsObjects;
using ShitForum.Mappers;

namespace ShitForum
{
    public static class DIModule
    {
        private static DbContextOptions GetDbOptions(IServiceProvider a) => new DbContextOptionsBuilder<ForumContext>()
            .UseSqlite(a.GetService<IConfiguration>().GetSection("DbPath").Get<string>()).Options;

        public static void Add(IServiceCollection services)
        {
            services.AddSingleton<IValidateImage, ValidateImage>();
            services.AddSingleton<IUploadMapper, UploadMapper>();
            services.AddSingleton<DbContextOptions>(GetDbOptions);
            services.AddSingleton<ICookieStorage, CookieStorage>();
            services.AddSingleton<IIsAdmin, IsAdmin.IsAdmin>();
            services.AddSingleton<IBannedImageLogger, BannedImageLogger.BannedImageLogger>();
            services.AddSingleton<IGetIp, GetIp.GetIp>();
            services.AddSingleton<IpHasherFactory>();
            services.AddSingleton<TripCodeHasher>();
            services.AddSingleton<AdminSettings>();
            services.AddSingleton<ForumSettings>();
        }
    }
}
