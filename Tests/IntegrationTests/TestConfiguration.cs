using Microsoft.Extensions.Configuration;

namespace IntegrationTests
{
    public static class TestConfiguration
    {
        public static IConfiguration GetConfig()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            return configurationBuilder.Build();
        }
    }
}