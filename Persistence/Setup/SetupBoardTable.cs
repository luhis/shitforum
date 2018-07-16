using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Setup
{
    public static class SetupBoardTable
    {
        public static void Setup(EntityTypeBuilder<Board> entity)
        {
            SetupEntity.Setup(entity);
            entity.Property(e => e.BoardName).IsRequired();
            entity.Property(e => e.BoardKey).IsRequired();
            entity.HasIndex(e => e.BoardKey).IsUnique();
        }
    }
}
