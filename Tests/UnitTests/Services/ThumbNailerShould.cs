using FluentAssertions;
using Tests.UnitTests.Tooling;
using ThumbNailer;
using Xunit;

namespace UnitTests.Services
{
    public class ThumbNailerShould
    {
        private readonly IThumbNailer thumbNailer;

        public ThumbNailerShould()
        {
            this.thumbNailer = new Thumbnailer(MockConfig.GetThumbNailerSettings());
        }

        [Fact]
        public void ThumbNailJpg()
        {
            var img = System.IO.File.ReadAllBytes("../../../Images/jezza.jpg");
            var thumb = this.thumbNailer.Make(img, ".jpg");
            thumb.Should().NotBeNull();
            thumb.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void ThumbnailWebm()
        {
            var img = System.IO.File.ReadAllBytes("../../../Images/dunno.webm");
            var thumb = this.thumbNailer.Make(img, ".webm");
            thumb.Should().NotBeNull();
            thumb.Length.Should().BeGreaterThan(0);
        }
    }
}
