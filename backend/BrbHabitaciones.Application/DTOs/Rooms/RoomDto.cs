using BrbHabitaciones.Application.DTOs.Amenities;
using BrbHabitaciones.Application.DTOs.Photos;

namespace BrbHabitaciones.Application.DTOs.Rooms;

public record RoomDto(
    Guid Id,
    Guid PropertyId,
    string PropertyName,
    string Title,
    string Description,
    int Capacity,
    decimal PricePerNight,
    bool IsActive,
    IEnumerable<PhotoDto> Photos,
    IEnumerable<AmenityDto> Amenities,
    DateTime CreatedAt);
