using FluentAssertions;
using Tests.UnitTests.Tooling;
using ThumbNailer;
using Xunit;

namespace UnitTests.Services
{
    public class ThumbNailerShould
    {
        private readonly IThumbNailer thumbnailer;

        public ThumbNailerShould()
        {
            var conf = MockConfig.Get();
            this.thumbnailer = new Thumbnailer(conf);
        }

        [Fact]
        public void ThumbNailJpg()
        {
            var img = System.IO.File.ReadAllBytes("../../../Images/jezza.jpg");
            var thumb = thumbnailer.Make(img, ".jpg");
            thumb.Should().NotBeNull();
            thumb.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void ThumbnailWebm()
        {
            var img = System.IO.File.ReadAllBytes("../../../Images/dunno.webm");
            var thumb = thumbnailer.Make(img, ".webm");
            thumb.Should().NotBeNull();
            thumb.Length.Should().BeGreaterThan(0);
        }
    }
}
