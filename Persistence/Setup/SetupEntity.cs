using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Setup
{
    public static class SetupEntity
    {
        public static void Setup<T>(EntityTypeBuilder<T> entity) where T : DomainBase
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).ValueGeneratedNever();
            ////entity.Property(c => c.Id).SetupAsCharGuid();
        }
    }
}