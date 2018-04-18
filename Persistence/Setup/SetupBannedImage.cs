using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Setup
{
    public static class SetupBannedImage
    {
        public static void Setup(EntityTypeBuilder<BannedImage> entity)
        {
            SetupEntity.Setup(entity);
            entity.Property(e => e.Reason).IsRequired();
            entity.Property(e => e.Hash).IsRequired();
        }
    }
}