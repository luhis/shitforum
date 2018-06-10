using System;
using Microsoft.Extensions.Configuration;

namespace Hashers
{
    public class IpHasherFactory
    {
        private readonly IConfiguration config;

        public IpHasherFactory(IConfiguration config)
        {
            this.config = config;
        }

        public IIpHasher GetHasher()
        {
            if (this.config.GetSection("IpHash:Enabled").Get<string>() == true.ToString())
            {
                return new SecureHasherHasher(this.config.GetSection("IpHash:Salt").Get<Guid>());
            }
            else
            {
                return new PassThroughHasher();
            }
        }
    }
}
