using AuthenticationEmbedder.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationEmbedder.DataBaseRequest
{
    public sealed class DataContext : DbContext
    {
        public DbSet<AuthModel> AuthModels { get; set; }
        public DbSet<LoginModel> LoginModels { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}