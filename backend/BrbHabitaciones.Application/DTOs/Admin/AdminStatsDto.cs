namespace BrbHabitaciones.Application.DTOs.Admin;

public record AdminStatsDto(
    int TotalUsers,
    int TotalProperties,
    int TotalRooms,
    int PendingProperties,
    int ReservationsThisMonth,
    decimal RevenueThisMonth,
    int ActiveReservations);
