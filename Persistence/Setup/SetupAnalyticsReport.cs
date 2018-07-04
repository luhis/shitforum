using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Setup
{
    public static class SetupAnalyticsReport
    {
        public static void Setup(EntityTypeBuilder<AnalyticsReport> entity)
        {
            SetupEntity.Setup(entity);
            entity.Property(e => e.Location).IsRequired();
            entity.Property(e => e.ThumbPrint).IsRequired();
            entity.Property(e => e.Time).IsRequired();
        }
    }
}