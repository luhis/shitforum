﻿using FluentAssertions;
using Hashers;
using Services.Dtos;
using UnitTests.Tooling;
using Xunit;

namespace UnitTests.Services
{
    public class TripCodeHasherShould
    {
        public TripCodeHasherShould()
        {
            var conf = this.mockConfig.GetTripCodeHasherSettings();
            this.hasher = new TripCodeHasher(conf);
        }

        private readonly TripCodeHasher hasher;
        private readonly MockConfig mockConfig = new MockConfig();

        [Fact]
        public void NotCrash()
        {
            hasher.Hash("aaa");
        }

        [Fact]
        public void AcceptNoCode()
        {
            var res = hasher.Hash("Matt M");
            res.Should().BeEquivalentTo(new TripCodedName("Matt M"));
        }

        [Fact]
        public void HashTheCode()
        {
            var res = hasher.Hash("Matt M #abcdefg");
            res.Should().BeEquivalentTo(new TripCodedName("Matt M !ZKOlfWJ8O3w8nL/7xlWFLecXJtn2y33KSoS4AWD84qo="));
        }

        [Fact]
        public void SecureHashTheCode()
        {
            var res = hasher.Hash("Matt M ##abcdefg");
            res.Should().BeEquivalentTo(new TripCodedName("Matt M !!bG++RzsrUa9zadnPIIbnXvqZaFQiFOq+R0j+6gZ3/BA="));
        }
    }
}
