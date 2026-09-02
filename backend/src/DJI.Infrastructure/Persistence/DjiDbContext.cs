using DJI.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DJI.Infrastructure.Persistence;

public class DjiDbContext(DbContextOptions<DjiDbContext> options) : DbContext(options)
{
    public DbSet<Manager> Managers => Set<Manager>();

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Sale> Sales => Set<Sale>();

    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(DjiDbContext).Assembly);
}
