using EnsureThat;

namespace ShitForum.Models
{
    public class LocationDetails
    {
        public LocationDetails(string name, int hits, int uniqueHits)
        {
            Name = EnsureArg.IsNotNull(name, nameof(name));
            Hits = hits;
            UniqueHits = uniqueHits;
        }

        public string Name { get; }
        public int Hits { get; }
        public int UniqueHits { get; }
    }
}