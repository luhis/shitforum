using EnsureThat;

namespace Domain.IpHash
{
    public class IpHash : IIpHash
    {
        public IpHash(string val)
        {
            Val = EnsureArg.IsNotNullOrWhiteSpace(val, nameof(val));
            Ensure.Bool.IsTrue(val.EndsWith("="), nameof(val));
            Ensure.String.HasLengthBetween(val, 44, 44, nameof(val));
        }

        public string Val { get; }
    }
}