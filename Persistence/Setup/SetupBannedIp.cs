using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Setup
{
    public static class SetupBannedIp
    {
        public static void Setup(EntityTypeBuilder<BannedIp> entity)
        {
            SetupEntity.Setup(entity);
            entity.Property(e => e.Reason).IsRequired();
            entity.Property(e => e.IpHash).IsRequired();
        }
    }
}
