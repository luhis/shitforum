using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Setup
{
    public static class SetupPostTable
    {
        public static void Setup(EntityTypeBuilder<Post> entity)
        {
            SetupEntity.Setup(entity);
            entity.Property(e => e.Comment).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.IpAddress).IsRequired();
            entity.Property(e => e.ThreadId).IsRequired();
            ////entity.Property(e => e.File).SetupAsCharGuid();
        }
    }
}