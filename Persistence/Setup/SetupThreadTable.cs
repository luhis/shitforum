using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Setup
{
    public static class SetupThreadTable
    {
        public static void Setup(EntityTypeBuilder<Thread> entity)
        {
            SetupEntity.Setup(entity);
            entity.Property(e => e.Subject).IsRequired();
            entity.Property(e => e.BoardId).IsRequired();
        }
    }
}