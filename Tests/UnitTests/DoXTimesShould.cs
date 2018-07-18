using FluentAssertions;
using Hashers;
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

        [Fact]
        public void ShouldAdd1OneTime()
        {
            var sut = Repeater.DoXTimes(0, i => i + 1, 1);
            sut.Should().Be(1);
        }
    }
}
