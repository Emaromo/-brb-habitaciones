namespace BrbHabitaciones.Application.DTOs.Admin;

public record AdminPropertyDto(
    Guid Id,
    string Name,
    string OwnerName,
    string Province,
    string City,
    string PropertyType,
    bool IsActive,
    bool IsApproved,
    int RoomCount,
    DateTime CreatedAt);
