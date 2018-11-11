using Microsoft.Extensions.Options;

namespace Hashers
{
    public class IpHasherFactory
    {
        private readonly IpHasherSettings config;

        public IpHasherFactory(IOptions<IpHasherSettings> config)
        {
            this.config = config.Value;
        }

        public IIpHasher GetHasher()
        {
            if (this.config.Enabled)
            {
                return new SecureHasherHasher(this.config.Salt);
            }
            else
            {
                return new PassThroughHasher();
            }
        }
    }
}
