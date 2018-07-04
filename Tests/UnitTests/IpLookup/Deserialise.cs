using FluentAssertions;
using Xunit;

namespace UnitTests.IpLookup
{
    public class DeserialiseShould
    {
        [Fact]
        public void Deserialise()
        {
            var sut = ExtremeIpLookup.ExtremeIpLookup.Deserialise(System.IO.File.ReadAllText("../../../IpLookup/Example.txt"));
            sut.City.Should().Be("Brighton");
            sut.Status.Should().Be("success");
        }
    }
}
