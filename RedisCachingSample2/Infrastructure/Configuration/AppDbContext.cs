using Microsoft.EntityFrameworkCore;
using RedisCachingSample2.Infrastructure.Entities;

namespace RedisCachingSample2.Infrastructure.Configuration
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) 
        {
            
        }

        public DbSet<Post> Posts { get; set; }
    }
}
