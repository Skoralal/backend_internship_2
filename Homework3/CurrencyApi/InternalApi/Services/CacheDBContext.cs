using Microsoft.EntityFrameworkCore;

namespace InternalApi.Services
{
    public class CacheDBContext : DbContext
    {
        public CacheDBContext(DbContextOptions options):base(options) { }
    }
}
