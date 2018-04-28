using FluentAssertions;
using ShitForum.Hasher;
using Xunit;

namespace UnitTests
{
    public class DoXTimesShould
    {
        [Fact]
        public void ShouldAdd1TenTimes()
        {
            var sut = Repeater.DoXTimes(0, i => i + 1, 10);
            sut.Should().Be(10);
        }
    }
}