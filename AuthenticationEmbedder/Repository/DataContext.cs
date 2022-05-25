using AuthenticationEmbedder.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationEmbedder.Repository;

public sealed class DataContext : DbContext
{
    public DbSet<EmailModel> EmailModels { get; set; }
    public DbSet<SiteLogin> LoginModels { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}