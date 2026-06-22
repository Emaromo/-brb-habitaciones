using BrbHabitaciones.Application.DTOs.Reservations;

namespace BrbHabitaciones.Application.Interfaces;

public interface IReservationService
{
    Task<IEnumerable<DateOnly>> GetBookedDatesAsync(Guid roomId, DateOnly from, DateOnly to);
    Task<ReservationDto> CreateAsync(CreateReservationRequest request, Guid clientId);
    Task<IEnumerable<ReservationDto>> GetMyAsync(Guid userId, string role);
    Task<ReservationDto?> GetByIdAsync(Guid id);
    Task<ReservationDto> CancelAsync(Guid id, Guid requesterId, string? reason = null);
}
