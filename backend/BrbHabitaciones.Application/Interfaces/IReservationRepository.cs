using BrbHabitaciones.Domain.Entities;

namespace BrbHabitaciones.Application.Interfaces;

public interface IReservationRepository
{
    Task<bool> HasOverlapAsync(Guid roomId, DateOnly checkIn, DateOnly checkOut);
    Task<IEnumerable<DateOnly>> GetBookedDatesAsync(Guid roomId, DateOnly from, DateOnly to);
    Task<Reservation> AddAsync(Reservation reservation);
    Task<Reservation?> GetByIdAsync(Guid id);
    Task<IEnumerable<Reservation>> GetByClientAsync(Guid clientId);
    Task<IEnumerable<Reservation>> GetByOwnerAsync(Guid ownerId);
    Task SaveChangesAsync();
}
