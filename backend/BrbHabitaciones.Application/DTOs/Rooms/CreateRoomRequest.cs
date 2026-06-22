namespace BrbHabitaciones.Application.DTOs.Rooms;

public record CreateRoomRequest(
    string Title,
    string Description,
    int Capacity,
    decimal PricePerNight);
