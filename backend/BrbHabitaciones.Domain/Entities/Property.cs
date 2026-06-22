using BrbHabitaciones.Domain.Enums;

namespace BrbHabitaciones.Domain.Entities;

public class Property
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public PropertyType PropertyType { get; set; } = PropertyType.Casa;
    public bool IsActive { get; set; } = true;
    public bool IsApproved { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public User Owner { get; set; } = null!;
    public ICollection<Room> Rooms { get; set; } = [];
    public ICollection<Photo> Photos { get; set; } = [];
}
