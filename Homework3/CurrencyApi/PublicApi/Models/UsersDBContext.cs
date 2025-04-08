using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace Fuse8.BackendInternship.PublicApi.Models
{
    public class UsersDBContext:DbContext
    {

        public UsersDBContext(DbContextOptions options) : base(options) { }
        public DbSet<FavoriteRateDBObject> FavoriteExchanges {  get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("user");
            modelBuilder.Entity<FavoriteRateDBObject>().HasIndex(e => new { e.SelectedCurrencyType, e.BaseCurrencyType }).IsUnique();
            modelBuilder.Entity<FavoriteRateDBObject>().HasIndex(e => new { e.Name });
        }
    }
}
