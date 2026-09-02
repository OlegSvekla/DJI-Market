using DJI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DJI.Infrastructure.Persistence.Configurations;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.HasKey(item => item.Id);

        builder.Property(item => item.UnitPrice).HasPrecision(18, 2);
        builder.Property(item => item.UnitCost).HasPrecision(18, 2);

        builder.HasOne(item => item.Sale)
            .WithMany(sale => sale.Items)
            .HasForeignKey(item => item.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(item => item.Product)
            .WithMany(product => product.SaleItems)
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(item => item.SaleId);

        builder.HasIndex(item => item.ProductId);
    }
}
