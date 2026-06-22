using BrbHabitaciones.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrbHabitaciones.Infrastructure.Data.Configurations;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.ToTable("Photos");
        builder.HasKey(ph => ph.Id);

        builder.Property(ph => ph.Url).IsRequired().HasMaxLength(500);
        builder.Property(ph => ph.PublicId).HasMaxLength(200);
        builder.Property(ph => ph.AltText).HasMaxLength(200);

        builder.HasOne(ph => ph.Room)
            .WithMany(r => r.Photos)
            .HasForeignKey(ph => ph.RoomId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.HasOne(ph => ph.Property)
            .WithMany(p => p.Photos)
            .HasForeignKey(ph => ph.PropertyId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
    }
}
