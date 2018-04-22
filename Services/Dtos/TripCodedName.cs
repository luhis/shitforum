using EnsureThat;

namespace Services.Dtos
{
    public class TripCodedName
    {
        public TripCodedName(string val)
        {
            this.Val = EnsureArg.IsNotNullOrEmpty(val);
        }

        public string Val { get; }
    }
}