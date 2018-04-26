using EnsureThat;

namespace Domain.IpHash
{
    public class IpUnHashed : IIpHash
    {
        public IpUnHashed(string val)
        {
            Val = EnsureArg.IsNotNullOrWhiteSpace(val, nameof(val));
        }

        public string Val { get; }
    }
}