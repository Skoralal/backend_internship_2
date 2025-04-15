using Microsoft.EntityFrameworkCore;

namespace Fuse8.BackendInternship.PublicApi.Models
{
    public class UsersDBContext : DbContext
    {
        public const string SchemaName = "user";
        public UsersDBContext(DbContextOptions options) : base(options) { }
        public DbSet<FavoriteRateDB> FavoriteExchanges { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(SchemaName);
            modelBuilder.Entity<FavoriteRateDB>().HasIndex(e => new { e.SelectedCurrencyType, e.BaseCurrencyType }).IsUnique();
            modelBuilder.Entity<FavoriteRateDB>().HasIndex(e => new { e.Name });
        }
    }
}
