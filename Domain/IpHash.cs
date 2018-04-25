using EnsureThat;

namespace Domain
{
    public class IpHash
    {
        public IpHash(string val)
        {
            Val = EnsureArg.IsNotNullOrWhiteSpace(val, nameof(val));
            Ensure.Bool.IsTrue(val.EndsWith("="), nameof(val));
        }

        public string Val { get; }
    }
}