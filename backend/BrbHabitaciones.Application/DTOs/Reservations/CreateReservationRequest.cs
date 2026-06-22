namespace BrbHabitaciones.Application.DTOs.Reservations;

public record CreateReservationRequest(
    Guid RoomId,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    int GuestCount);
