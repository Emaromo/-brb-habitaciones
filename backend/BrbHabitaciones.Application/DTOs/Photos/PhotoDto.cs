namespace BrbHabitaciones.Application.DTOs.Photos;

public record PhotoDto(
    Guid Id,
    string Url,
    string PublicId,
    bool IsCover,
    int DisplayOrder);
