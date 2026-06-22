namespace BrbHabitaciones.Application.DTOs.Rooms;

public record RoomSummaryDto(
    Guid Id,
    string Title,
    int Capacity,
    decimal PricePerNight,
    string? CoverPhotoUrl);
