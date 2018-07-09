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
            sut.Message.Should().BeNull();
            sut.Region.Should().Be("Brighton and Hove");
            sut.CountryCode.Should().Be("GB");
            sut.Country.Should().Be("United Kingdom");
            sut.Continent.Should().Be("Europe");
            sut.BusinessName.Should().Be("");
            sut.BusinessWebsite.Should().Be("");
            sut.IpType.Should().Be("Residential");
            sut.IpName.Should().Be("deadbeef.bb.sly.com");
            sut.ISP.Should().Be("Sly Broadband");
            sut.Query.Should().Be("1.1.1.1");
            sut.Lat.Should().Be(50.0);
            sut.Lon.Should().Be(-0.0);
            sut.Org.Should().Be("Sly UK Limited");
        }
    }
}
