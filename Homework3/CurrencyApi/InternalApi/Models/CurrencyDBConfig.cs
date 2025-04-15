using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace InternalApi.Models
{
    public class CurrencyDBConfig : IEntityTypeConfiguration<CurrencyDB>
    {
        public void Configure(EntityTypeBuilder<CurrencyDB> builder)
        {
            builder.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
        }
    }
}
