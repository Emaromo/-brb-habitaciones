namespace BrbHabitaciones.Domain.Entities;

public class Amenity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    public ICollection<RoomAmenity> RoomAmenities { get; set; } = [];
}
