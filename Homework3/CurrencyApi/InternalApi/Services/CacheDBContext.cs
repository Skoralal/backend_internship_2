using InternalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InternalApi.Services
{
    public class CacheDBContext : DbContext
    {
        public CacheDBContext(DbContextOptions options):base(options) { }

        public DbSet<CurrencyDateBaseObject> ExchangeRates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("cur");

            modelBuilder.Entity<CurrencyDateBaseObject>()
                .HasKey(e => new { e.Code, e.ActualityTime });
        }
    }
}
