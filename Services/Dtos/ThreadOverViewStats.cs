using EnsureThat;

namespace Services.Dtos
{
    public class ThreadOverViewStats
    {
        public ThreadOverViewStats(int postCount, int imageCount)
        {
            PostCount = EnsureArg.IsGte(postCount, 0, nameof(PostCount));
            ImageCount = EnsureArg.IsGte(imageCount, 0, nameof(ImageCount));
        }

        public int PostCount { get; }

        public int ImageCount { get; }
    }
}