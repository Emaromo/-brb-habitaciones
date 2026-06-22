namespace BrbHabitaciones.Domain.Entities;

public class Room
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PropertyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Capacity { get; set; } = 1;
    public decimal PricePerNight { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public Property Property { get; set; } = null!;
    public ICollection<Photo> Photos { get; set; } = [];
    public ICollection<RoomAmenity> RoomAmenities { get; set; } = [];
}
