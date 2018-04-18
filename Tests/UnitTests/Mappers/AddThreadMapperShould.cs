using System;
using System.Net;
using Domain;
using FluentAssertions;
using ShitForum.Mappers;
using ShitForum.Models;
using Xunit;

namespace UnitTests.Mappers
{
    public class AddThreadMapperShould
    {
        [Fact]
        public void AcceptNullSubject()
        {
            var mapped = AddThreadMapper.Map(new AddThread(Guid.NewGuid(), null, null, null, "comment", null));
            mapped.Subject.Should().Be("");
        }
    }
}