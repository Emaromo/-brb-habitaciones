namespace BrbHabitaciones.Domain.Entities;

public class Availability
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RoomId { get; set; }
    public Guid ReservationId { get; set; }
    public DateOnly Date { get; set; }

    public Room Room { get; set; } = null!;
    public Reservation Reservation { get; set; } = null!;
}
