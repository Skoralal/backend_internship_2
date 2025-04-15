using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fuse8.BackendInternship.PublicApi.Models
{
    public class FavoriteRateDBConfig : IEntityTypeConfiguration<FavoriteRateDB>
    {
        public void Configure(EntityTypeBuilder<FavoriteRateDB> builder)
        {
            builder.Property(x => x.Id).HasDefaultValue("uuid_generate_v4()");
        }
    }
}
