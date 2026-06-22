using BrbHabitaciones.Application.DTOs.Properties;
using BrbHabitaciones.Application.Interfaces;
using BrbHabitaciones.Domain.Entities;
using BrbHabitaciones.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BrbHabitaciones.Infrastructure.Repositories;

public class PropertyRepository(AppDbContext db) : IPropertyRepository
{
    public async Task<(IEnumerable<Property> Items, int TotalCount)> SearchAsync(PropertySearchQuery query)
    {
        var q = db.Properties
            .Include(p => p.Owner)
            .Include(p => p.Rooms)
            .Include(p => p.Photos)
            .Where(p => p.DeletedAt == null && p.IsActive && p.IsApproved);

        if (!string.IsNullOrWhiteSpace(query.Province))
            q = q.Where(p => p.Province == query.Province);

        if (!string.IsNullOrWhiteSpace(query.City))
            q = q.Where(p => p.City.ToLower().Contains(query.City.ToLower()));

        if (query.MinCapacity.HasValue)
            q = q.Where(p => p.Rooms.Any(r => r.Capacity >= query.MinCapacity.Value && r.IsActive && r.DeletedAt == null));

        if (query.MaxPrice.HasValue)
            q = q.Where(p => p.Rooms.Any(r => r.PricePerNight <= query.MaxPrice.Value && r.IsActive && r.DeletedAt == null));

        var total = await q.CountAsync();
        var items = await q
            .OrderByDescending(p => p.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Property?> GetByIdAsync(Guid id) =>
        await db.Properties
            .Include(p => p.Owner)
            .Include(p => p.Rooms.Where(r => r.DeletedAt == null))
                .ThenInclude(r => r.Photos)
            .Include(p => p.Rooms.Where(r => r.DeletedAt == null))
                .ThenInclude(r => r.RoomAmenities)
                    .ThenInclude(ra => ra.Amenity)
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);

    public async Task<IEnumerable<Property>> GetByOwnerAsync(Guid ownerId) =>
        await db.Properties
            .Include(p => p.Owner)
            .Include(p => p.Rooms)
            .Include(p => p.Photos)
            .Where(p => p.OwnerId == ownerId && p.DeletedAt == null)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

    public async Task<Property> AddAsync(Property property)
    {
        db.Properties.Add(property);
        return property;
    }

    public async Task SaveChangesAsync() => await db.SaveChangesAsync();
}
