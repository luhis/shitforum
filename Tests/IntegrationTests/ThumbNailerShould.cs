using FluentAssertions;
using ThumbNailer;
using Xunit;

namespace IntegrationTests
{
    public class ThumbNailerShould
    {
        private readonly IThumbNailer thumbNailer;
        private readonly MockConfig mockConfig = new MockConfig();

        public ThumbNailerShould()
        {
            this.thumbNailer = new ThumbNailer.ThumbNailer(this.mockConfig.GetThumbNailerSettings());
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
