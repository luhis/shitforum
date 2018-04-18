using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Setup
{
    public static class SetupFileTable
    {
        public static void Setup(EntityTypeBuilder<File> entity)
        {
            SetupEntity.Setup(entity);
            entity.Property(e => e.Data).IsRequired();
            entity.Property(e => e.FileName).IsRequired();
            entity.Property(e => e.MimeType).IsRequired();
            entity.Property(e => e.ThumbNailJpeg).IsRequired();
        }
    }
}