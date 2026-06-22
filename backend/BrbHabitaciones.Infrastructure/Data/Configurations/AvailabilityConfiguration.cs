using BrbHabitaciones.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrbHabitaciones.Infrastructure.Data.Configurations;

public class AvailabilityConfiguration : IEntityTypeConfiguration<Availability>
{
    public void Configure(EntityTypeBuilder<Availability> builder)
    {
        builder.ToTable("Availability");
        builder.HasKey(a => a.Id);

        builder.HasOne(a => a.Room)
            .WithMany()
            .HasForeignKey(a => a.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Reservation)
            .WithMany(r => r.Availability)
            .HasForeignKey(a => a.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Un cuarto no puede tener la misma fecha bloqueada dos veces
        builder.HasIndex(a => new { a.RoomId, a.Date }).IsUnique();
    }
}
