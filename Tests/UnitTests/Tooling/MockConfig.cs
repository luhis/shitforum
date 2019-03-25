using System;
using Hashers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ShitForum;
using ThumbNailer;

namespace UnitTests.Tooling
{
    public class MockConfig
    {
        private readonly IConfiguration configuration;

        public MockConfig()
        {
            this.configuration = new ConfigurationBuilder()
                .AddJsonFile("./appsettings.json", optional: false, reloadOnChange: false)
                .Build();
        }

        public IOptions<AdminSettingsRaw> GetAdminSettings()
        {
            var s = new AdminSettingsRaw();
            this.configuration.Bind(s);
            return Options.Create(s);
        }

        public IOptions<IpHasherSettings> GetHasherSettings()
        {
            var s = new IpHasherSettings();
            this.configuration.GetSection("IpHash").Bind(s);
            return Options.Create(s);
        }

        public IOptions<ThumbNailerSettings> GetThumbNailerSettings()
        {
            var s = new ThumbNailerSettings();
            this.configuration.Bind(s);
            return Options.Create(s);
        }

        public IOptions<TripCodeHasherSettings> GetTripCodeHasherSettings()
        {
            var s = new TripCodeHasherSettings();
            this.configuration.Bind(s);
            return Options.Create(s);
        }
    }
}
