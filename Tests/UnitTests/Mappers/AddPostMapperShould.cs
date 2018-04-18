using System;
using System.Net;
using Domain;
using FluentAssertions;
using Services;
using Services.Dtos;
using ShitForum.Mappers;
using ShitForum.Models;
using Xunit;

namespace UnitTests.Mappers
{
    public class AddPostMapperShould
    {
        private readonly IpHash loopBack = new IpHash(IPAddress.Loopback.ToString());

        [Fact]
        public void AcceptNullOptions()
        {
            var mapped = AddPostMapper.Map(new AddPost(Guid.NewGuid(), null, null, "comment", null), new TripCodedName("matt"), loopBack);
            mapped.IsSage.Should().Be(false);
        }
    }
}
