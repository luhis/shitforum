using System.Collections.Generic;
using EnsureThat;

namespace ShitForum.Models
{
    public class AnalyticsViewModel
    {
        public AnalyticsViewModel(IReadOnlyList<LocationDetails> hits)
        {
            Hits = EnsureArg.IsNotNull(hits, nameof(hits));
        }

        public IReadOnlyList<LocationDetails> Hits { get; }
    }
}
