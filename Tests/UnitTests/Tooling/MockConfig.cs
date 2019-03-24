using System;
using System.Collections.Generic;
using Hashers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ShitForum;
using ThumbNailer;

namespace UnitTests.Tooling
{
    public static class MockConfig
    {
        private static readonly IConfiguration Configuration = new ConfigurationBuilder()
            .AddJsonFile("./appsettings.json", optional: false, reloadOnChange: false).Build();

        public static IOptions<AdminSettingsRaw> GetAdminSettings()
        {
            var s = new AdminSettingsRaw();
            Configuration.Bind(s);
            return Options.Create(s);
        }

        public static IOptions<IpHasherSettings> GetHasherSettings()
        {
            var s = new IpHasherSettings();
            Configuration.GetSection("IpHash").Bind(s);
            return Options.Create(s);
        }

        public static IOptions<ThumbNailerSettings> GetThumbNailerSettings()
        {
            var s = new ThumbNailerSettings();
            Configuration.Bind(s);
            return Options.Create(s);
        }

        public static IOptions<TripCodeHasherSettings> GetTripCodeHasherSettings()
        {
            var s = new TripCodeHasherSettings();
            Configuration.Bind(s);
            return Options.Create(s);
        }
    }
}
