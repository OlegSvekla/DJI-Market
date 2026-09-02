using DJI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DJI.Infrastructure.Persistence.Configurations;

public class ManagerConfiguration : IEntityTypeConfiguration<Manager>
{
    public void Configure(EntityTypeBuilder<Manager> builder)
    {
        builder.HasKey(manager => manager.Id);

        builder.Property(manager => manager.FirstName).HasMaxLength(64).IsRequired();
        builder.Property(manager => manager.LastName).HasMaxLength(64).IsRequired();
        builder.Property(manager => manager.Team).HasMaxLength(64).IsRequired();
        builder.Property(manager => manager.Position).HasMaxLength(64).IsRequired();
        builder.Property(manager => manager.AvatarColor).HasMaxLength(7).IsRequired();
    }
}
