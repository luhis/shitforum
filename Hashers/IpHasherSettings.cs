using System;

namespace Hashers
{
    public class IpHasherSettings
    {
        public bool Enabled { get; set; }
        public Guid Salt { get; set; }
    }
}