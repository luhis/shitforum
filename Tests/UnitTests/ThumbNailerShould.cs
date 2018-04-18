using FluentAssertions;
using ShitForum;
using Xunit;

namespace UnitTests
{
    public class ThumbNailerShould
    {
        [Fact]
        public void ThumbNailJpg()
        {
            var img = System.IO.File.ReadAllBytes("../../../Images/jezza.jpg");
            var thumb = Thumbnailer.Make(img);
            thumb.Should().NotBeNull();
        }

        [Fact(Skip = "I will fix this i swear")]
        public void ThumbnailWebm()
        {
            var img = System.IO.File.ReadAllBytes("../../../Images/dunno.webm");
            var thumb = Thumbnailer.Make(img);
            thumb.Should().NotBeNull();
        }
    }
}