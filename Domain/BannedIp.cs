using EnsureThat;
using System;

namespace Domain
{
    public class BannedIp : DomainBase
    {
        public BannedIp(Guid id, string ipHash, string reason, DateTime expiry): base(id)
        {
            IpHash = EnsureArg.IsNotNull(ipHash, nameof(ipHash));
            Reason = EnsureArg.IsNotNull(reason, nameof(reason));
            Expiry = EnsureArg.IsNotNull(expiry, nameof(expiry));
        }

        public BannedIp()
        {
        }

        public string IpHash { get; private set; }
        public string Reason { get; private set; }
        public DateTime Expiry { get; private set; }
    }
}
