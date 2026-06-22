using BrbHabitaciones.Application.Interfaces;
using BrbHabitaciones.Domain.Entities;
using BrbHabitaciones.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BrbHabitaciones.Infrastructure.Repositories;

public class RoomRepository(AppDbContext db) : IRoomRepository
{
    public async Task<IEnumerable<Room>> GetByPropertyAsync(Guid propertyId) =>
        await db.Rooms
            .Include(r => r.Property)
            .Include(r => r.Photos)
            .Include(r => r.RoomAmenities).ThenInclude(ra => ra.Amenity)
            .Where(r => r.PropertyId == propertyId && r.DeletedAt == null && r.IsActive)
            .OrderBy(r => r.PricePerNight)
            .ToListAsync();

    public async Task<Room?> GetByIdAsync(Guid id) =>
        await db.Rooms
            .Include(r => r.Property)
            .Include(r => r.Photos)
            .Include(r => r.RoomAmenities).ThenInclude(ra => ra.Amenity)
            .FirstOrDefaultAsync(r => r.Id == id && r.DeletedAt == null);

    public async Task<bool> IsPropertyOwnerAsync(Guid propertyId, Guid userId) =>
        await db.Properties.AnyAsync(p => p.Id == propertyId && p.OwnerId == userId && p.DeletedAt == null);

    public async Task<Room> AddAsync(Room room)
    {
        db.Rooms.Add(room);
        return room;
    }

    public async Task SaveChangesAsync() => await db.SaveChangesAsync();
}
