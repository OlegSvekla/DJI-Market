using DJI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DJI.Infrastructure.Persistence.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.HasKey(sale => sale.Id);

        builder.Property(sale => sale.Number).HasMaxLength(16).IsRequired();
        builder.Property(sale => sale.Status).HasConversion<int>();

        builder.HasOne(sale => sale.Manager)
            .WithMany(manager => manager.Sales)
            .HasForeignKey(sale => sale.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sale => sale.Customer)
            .WithMany(customer => customer.Sales)
            .HasForeignKey(sale => sale.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(sale => sale.Number).IsUnique();

        builder.HasIndex(sale => new { sale.SaleDate, sale.Status });

        builder.HasIndex(sale => new { sale.ManagerId, sale.SaleDate });
    }
}
