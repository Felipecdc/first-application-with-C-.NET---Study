using Microsoft.EntityFrameworkCore;
using first_dotnet_api.Models;

namespace first_dotnet_api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TaskItem> TaskItems { get; set; } = null!;
        public DbSet<User> Users { get; set; }
    }
}
