using BrbHabitaciones.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrbHabitaciones.Infrastructure.Data.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("Properties");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).HasMaxLength(2000);
        builder.Property(p => p.Province).IsRequired().HasMaxLength(100);
        builder.Property(p => p.City).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Address).HasMaxLength(300);
        builder.Property(p => p.PropertyType).HasConversion<string>();

        builder.HasOne(p => p.Owner)
            .WithMany()
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.Province);
        builder.HasIndex(p => p.City);
        builder.HasIndex(p => new { p.Province, p.City });
    }
}
