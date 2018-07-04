using EnsureThat;
using System;

namespace Domain
{
    public class BannedImage : DomainBase
    {
        public BannedImage(Guid id,string hash, string reason): base(id)
        {
            Hash = EnsureArg.IsNotNull(hash, nameof(hash));
            Reason = EnsureArg.IsNotNull(reason, nameof(reason));
        }

        public BannedImage()
        {
        }

        public string Hash { get; private set; }
        public string Reason { get; private set; }
    }
}
