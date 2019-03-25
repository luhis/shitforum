using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ThumbNailer;

namespace IntegrationTests
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

        public IOptions<ThumbNailerSettings> GetThumbNailerSettings()
        {
            var s = new ThumbNailerSettings();
            this.configuration.Bind(s);
            return Options.Create(s);
        }
    }
}
