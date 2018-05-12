using EnsureThat;

namespace Services.Dtos
{
    public class ThreadStats
    {
        public ThreadStats(int replies, int images, int posters, int page)
        {
            Replies = EnsureArg.IsGte(replies, 0, nameof(replies));
            Images = EnsureArg.IsGte(images, 0, nameof(images));
            Posters = EnsureArg.IsGte(posters, 0, nameof(posters));
            Page = EnsureArg.IsGte(page, 0, nameof(page));
        }

        public int Replies { get; }
        public int Images { get; }
        public int Posters { get; }
        public int Page { get; }
    }
}
