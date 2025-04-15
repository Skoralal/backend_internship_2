using Microsoft.EntityFrameworkCore;

namespace Fuse8.BackendInternship.PublicApi.Models
{
    public class CacheDBContext : DbContext
    {
        public CacheDBContext(DbContextOptions options) : base(options)
        {
        }
    }
}
