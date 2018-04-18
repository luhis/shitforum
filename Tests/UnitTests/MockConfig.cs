using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using System.Collections.Generic;

namespace UnitTests
{
    public static class MockConfig
    {
        public static IConfigurationRoot Get()
        {
            var builder = new ConfigurationBuilder();
            var a = new MemoryConfigurationSource
            {
                InitialData = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("TripCodeSalt", "20849c9c-6960-44a6-9f2d-51cc17ef0ee4"),
                    new KeyValuePair<string, string>("IpHash:Enable", "true"),
                    new KeyValuePair<string, string>("IpHash:Salt", "20849c9c-6960-44a6-9f2d-51cc17ef0ee5"),
                }
            };
            builder.Add(a);

            return builder.Build();
        }
    }
}
