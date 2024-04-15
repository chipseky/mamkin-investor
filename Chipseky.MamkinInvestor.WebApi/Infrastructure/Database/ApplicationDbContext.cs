using Chipseky.MamkinInvestor.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chipseky.MamkinInvestor.WebApi.Infrastructure.Database;

public class ApplicationDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(builder =>
        {
            builder.ToTable("orders");
        });
    }
}