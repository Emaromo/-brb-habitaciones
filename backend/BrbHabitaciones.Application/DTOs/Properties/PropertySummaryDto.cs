namespace BrbHabitaciones.Application.DTOs.Properties;

public record PropertySummaryDto(
    Guid Id,
    string Name,
    string Province,
    string City,
    string PropertyType,
    int RoomCount,
    decimal? MinPricePerNight,
    string? CoverPhotoUrl,
    DateTime CreatedAt);
