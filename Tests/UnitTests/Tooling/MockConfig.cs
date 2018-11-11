using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using System.Collections.Generic;
using Hashers;
using Microsoft.Extensions.Options;
using ShitForum;
using ThumbNailer;

namespace Tests.UnitTests.Tooling
{
    public static class MockConfig
    {
        public static IOptions<AdminSettingsRaw> GetAdminSettings()
        {
            var s = new AdminSettingsRaw() { Gods = new List<Guid>(){ Guid.Parse("3c68640c-5759-4be2-ab50-d6cd5cd6ba68") } };
            return Options.Create(s);
        }

        public static IOptions<IpHasherSettings> GetHasherSettings()
        {
            var s = new IpHasherSettings() { Enabled = true, Salt = Guid.Parse("20849c9c-6960-44a6-9f2d-51cc17ef0ee5") };
            return Options.Create(s);
        }

        public static IOptions<ThumbNailerSettings> GetThumbNailerSettings()
        {
            var s = new ThumbNailerSettings() { FfmpegLocation = "C:\\ProgramData\\chocolatey\\lib\\ffmpeg\\tools\\ffmpeg\\bin\\ffmpeg.exe" };
            return Options.Create(s);
        }

        public static IOptions<TripCodeHasherSettings> GetTripCodeHasherSettings()
        {
            var s = new TripCodeHasherSettings() { TripCodeSalt = Guid.Parse("20849c9c-6960-44a6-9f2d-51cc17ef0ee4") };
            return Options.Create(s);
        }
    }
}
