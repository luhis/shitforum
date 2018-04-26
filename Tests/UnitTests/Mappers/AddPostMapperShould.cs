using System;
using System.Net;
using Domain.IpHash;
using FluentAssertions;
using Services.Dtos;
using ShitForum.Mappers;
using ShitForum.Models;
using Xunit;

namespace UnitTests.Mappers
{
    public class AddPostMapperShould
    {
        private readonly IpUnHashed loopBack = new IpUnHashed(IPAddress.Loopback.ToString());

        [Fact]
        public void AcceptNullOptions()
        {
            var mapped = AddPostMapper.Map(new AddPost(Guid.NewGuid(), null, null, "comment", null), new TripCodedName("matt"), loopBack);
            mapped.IsSage.Should().Be(false);
        }
    }
}
