using BrbHabitaciones.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrbHabitaciones.Infrastructure.Data.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.TotalPrice).HasPrecision(10, 2);
        builder.Property(r => r.Status).HasConversion<string>();
        builder.Property(r => r.CancellationReason).HasMaxLength(500);

        builder.HasOne(r => r.Room)
            .WithMany()
            .HasForeignKey(r => r.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Client)
            .WithMany()
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => r.ClientId);
        builder.HasIndex(r => r.RoomId);
        builder.HasIndex(r => new { r.RoomId, r.CheckInDate, r.CheckOutDate });
    }
}
