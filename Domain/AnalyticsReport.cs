using System;

namespace Domain
{
    public sealed class AnalyticsReport : DomainBase
    {
        public AnalyticsReport(Guid id, DateTime time, string location, string thumbPrint) : base(id)
        {
            Time = time;
            Location = location;
            ThumbPrint = thumbPrint;
        }

        public AnalyticsReport()
        {
        }

        public DateTime Time { get; private set; }
        public string Location { get; private set; }
        public string ThumbPrint { get; private set; }
    }
}
