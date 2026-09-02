using DJI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DJI.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name).HasMaxLength(128).IsRequired();
        builder.Property(product => product.Sku).HasMaxLength(32).IsRequired();

        builder.Property(product => product.ListPrice).HasPrecision(18, 2);
        builder.Property(product => product.BaseCost).HasPrecision(18, 2);

        builder.HasOne(product => product.Category)
            .WithMany(category => category.Products)
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(product => product.Sku).IsUnique();

        builder.HasIndex(product => product.CategoryId);
    }
}
