using DJI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DJI.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(customer => customer.Id);

        builder.Property(customer => customer.Name).HasMaxLength(128).IsRequired();
        builder.Property(customer => customer.Company).HasMaxLength(128).IsRequired();
        builder.Property(customer => customer.Segment).HasConversion<int>();
    }
}
