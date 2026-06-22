namespace BrbHabitaciones.Application.DTOs.Amenities;

public record AmenityDto(
    Guid Id,
    string Name,
    string Icon,
    string Category);
