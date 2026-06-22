namespace BrbHabitaciones.Application.DTOs.Reservations;

public record ReservationDto(
    Guid Id,
    Guid RoomId,
    string RoomTitle,
    Guid PropertyId,
    string PropertyName,
    string PropertyCity,
    Guid ClientId,
    string ClientName,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    int GuestCount,
    int Nights,
    decimal TotalPrice,
    string Status,
    string? CancellationReason,
    DateTime CreatedAt);
