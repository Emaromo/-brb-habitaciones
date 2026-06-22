using BrbHabitaciones.Application.DTOs.Photos;
using BrbHabitaciones.Application.DTOs.Rooms;

namespace BrbHabitaciones.Application.DTOs.Properties;

public record PropertyDto(
    Guid Id,
    Guid OwnerId,
    string OwnerName,
    string Name,
    string Description,
    string Province,
    string City,
    string Address,
    string PropertyType,
    bool IsActive,
    bool IsApproved,
    IEnumerable<RoomSummaryDto> Rooms,
    IEnumerable<PhotoDto> Photos,
    DateTime CreatedAt);
