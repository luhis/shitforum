using EnsureThat;
using System;

namespace Domain
{
    public sealed class Ban : DomainBase
    {
        public Ban(Guid id, DateTime expiry, string ipAdress, string reason) : base(id)
        {
            Expiry = EnsureArg.IsNotDefault(expiry, nameof(expiry));
            IpAdress = EnsureArg.IsNotNull(ipAdress, nameof(ipAdress));
            this.Reason = EnsureArg.IsNotNull(reason, nameof(reason));
        }

        public DateTime Expiry { get; private set; }
        public string IpAdress { get; private set; }
        public string Reason { get; private set; }
    }
}