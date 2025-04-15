using InternalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InternalApi.Services
{
    public class CacheDBContext : DbContext
    {
        /// <summary>
        /// name of the chema used for cache tables
        /// </summary>
        public const string SchemaName = "cur";
        public CacheDBContext(DbContextOptions options) : base(options) { }
        /// <summary>
        /// table with all the exchange rates
        /// </summary>
        public DbSet<CurrencyDB> ExchangeRates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");
            modelBuilder.HasDefaultSchema(SchemaName);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
