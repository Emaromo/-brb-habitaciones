namespace BrbHabitaciones.Application.DTOs.Rooms;

public record UpdateRoomRequest(
    string? Title,
    string? Description,
    int? Capacity,
    decimal? PricePerNight,
    bool? IsActive);
