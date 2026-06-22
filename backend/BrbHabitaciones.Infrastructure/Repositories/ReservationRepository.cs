using BrbHabitaciones.Application.Interfaces;
using BrbHabitaciones.Domain.Entities;
using BrbHabitaciones.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BrbHabitaciones.Infrastructure.Repositories;

public class ReservationRepository(AppDbContext db) : IReservationRepository
{
    public async Task<bool> HasOverlapAsync(Guid roomId, DateOnly checkIn, DateOnly checkOut) =>
        await db.Availability.AnyAsync(a =>
            a.RoomId == roomId &&
            a.Date >= checkIn &&
            a.Date < checkOut);

    public async Task<IEnumerable<DateOnly>> GetBookedDatesAsync(Guid roomId, DateOnly from, DateOnly to) =>
        await db.Availability
            .Where(a => a.RoomId == roomId && a.Date >= from && a.Date <= to)
            .Select(a => a.Date)
            .OrderBy(d => d)
            .ToListAsync();

    public Task<Reservation> AddAsync(Reservation reservation)
    {
        db.Reservations.Add(reservation);
        return Task.FromResult(reservation);
    }

    public async Task<Reservation?> GetByIdAsync(Guid id) =>
        await db.Reservations
            .Include(r => r.Room).ThenInclude(room => room.Property)
            .Include(r => r.Client)
            .Include(r => r.Availability)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<IEnumerable<Reservation>> GetByClientAsync(Guid clientId) =>
        await db.Reservations
            .Include(r => r.Room).ThenInclude(room => room.Property)
            .Include(r => r.Client)
            .Where(r => r.ClientId == clientId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Reservation>> GetByOwnerAsync(Guid ownerId) =>
        await db.Reservations
            .Include(r => r.Room).ThenInclude(room => room.Property)
            .Include(r => r.Client)
            .Where(r => r.Room.Property.OwnerId == ownerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task SaveChangesAsync() => await db.SaveChangesAsync();
}
