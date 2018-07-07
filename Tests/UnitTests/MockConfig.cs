using System;
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
                    KeyValuePair.Create("TripCodeSalt", "20849c9c-6960-44a6-9f2d-51cc17ef0ee4"),
                    KeyValuePair.Create("IpHash:Enable", "true"),
                    KeyValuePair.Create("IpHash:Salt", "20849c9c-6960-44a6-9f2d-51cc17ef0ee5"),
                    KeyValuePair.Create("FfmpegLocation", "C:\\ProgramData\\chocolatey\\lib\\ffmpeg\\tools\\ffmpeg\\bin\\ffmpeg.exe"),
                    KeyValuePair.Create("Gods:0", "3c68640c-5759-4be2-ab50-d6cd5cd6ba68")
                }
            };
            builder.Add(a);

            return builder.Build();
        }
    }
}
