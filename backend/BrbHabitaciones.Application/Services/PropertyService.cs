using BrbHabitaciones.Application.DTOs.Common;
using BrbHabitaciones.Application.DTOs.Photos;
using BrbHabitaciones.Application.DTOs.Properties;
using BrbHabitaciones.Application.DTOs.Rooms;
using BrbHabitaciones.Application.Interfaces;
using BrbHabitaciones.Domain.Entities;
using BrbHabitaciones.Domain.Enums;

namespace BrbHabitaciones.Application.Services;

public class PropertyService(IPropertyRepository repo) : IPropertyService
{
    public async Task<PagedResult<PropertySummaryDto>> SearchAsync(PropertySearchQuery query)
    {
        var (items, total) = await repo.SearchAsync(query);
        var dtos = items.Select(MapToSummary);
        return new PagedResult<PropertySummaryDto>(dtos, total, query.Page, query.PageSize);
    }

    public async Task<PropertyDto?> GetByIdAsync(Guid id)
    {
        var p = await repo.GetByIdAsync(id);
        return p is null ? null : MapToDto(p);
    }

    public async Task<IEnumerable<PropertySummaryDto>> GetByOwnerAsync(Guid ownerId)
    {
        var items = await repo.GetByOwnerAsync(ownerId);
        return items.Select(MapToSummary);
    }

    public async Task<PropertyDto> CreateAsync(CreatePropertyRequest request, Guid ownerId)
    {
        if (!Enum.TryParse<PropertyType>(request.PropertyType, ignoreCase: true, out var type))
            type = PropertyType.Otro;

        var property = new Property
        {
            OwnerId = ownerId,
            Name = request.Name,
            Description = request.Description,
            Province = request.Province,
            City = request.City,
            Address = request.Address,
            PropertyType = type,
        };

        var created = await repo.AddAsync(property);
        await repo.SaveChangesAsync();

        var full = await repo.GetByIdAsync(created.Id)
            ?? throw new InvalidOperationException("Error al recuperar la propiedad creada.");
        return MapToDto(full);
    }

    public async Task<PropertyDto> UpdateAsync(Guid id, UpdatePropertyRequest request, Guid requesterId, bool isAdmin = false)
    {
        var p = await repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Propiedad no encontrada.");

        if (p.OwnerId != requesterId && !isAdmin)
            throw new UnauthorizedAccessException("No tenés permiso para modificar esta propiedad.");

        if (request.Name is not null) p.Name = request.Name;
        if (request.Description is not null) p.Description = request.Description;
        if (request.Province is not null) p.Province = request.Province;
        if (request.City is not null) p.City = request.City;
        if (request.Address is not null) p.Address = request.Address;
        if (request.IsActive.HasValue) p.IsActive = request.IsActive.Value;
        if (request.PropertyType is not null && Enum.TryParse<PropertyType>(request.PropertyType, ignoreCase: true, out var type))
            p.PropertyType = type;

        p.UpdatedAt = DateTime.UtcNow;
        await repo.SaveChangesAsync();

        var updated = await repo.GetByIdAsync(id)!;
        return MapToDto(updated!);
    }

    public async Task DeleteAsync(Guid id, Guid requesterId, bool isAdmin = false)
    {
        var p = await repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Propiedad no encontrada.");

        if (p.OwnerId != requesterId && !isAdmin)
            throw new UnauthorizedAccessException("No tenés permiso para eliminar esta propiedad.");

        p.DeletedAt = DateTime.UtcNow;
        p.IsActive = false;
        await repo.SaveChangesAsync();
    }

    private static PropertySummaryDto MapToSummary(Property p)
    {
        var activeRooms = p.Rooms.Where(r => r.IsActive && r.DeletedAt == null).ToList();
        return new PropertySummaryDto(
            p.Id,
            p.Name,
            p.Province,
            p.City,
            p.PropertyType.ToString(),
            activeRooms.Count,
            activeRooms.Select(r => r.PricePerNight).OrderBy(x => x).Cast<decimal?>().FirstOrDefault(),
            p.Photos.Where(ph => ph.PropertyId == p.Id).OrderBy(ph => ph.DisplayOrder).FirstOrDefault()?.Url,
            p.CreatedAt);
    }

    private static PropertyDto MapToDto(Property p) => new(
        p.Id,
        p.OwnerId,
        $"{p.Owner.FirstName} {p.Owner.LastName}",
        p.Name,
        p.Description,
        p.Province,
        p.City,
        p.Address,
        p.PropertyType.ToString(),
        p.IsActive,
        p.IsApproved,
        p.Rooms
            .Where(r => r.IsActive && r.DeletedAt == null)
            .Select(r => new RoomSummaryDto(
                r.Id,
                r.Title,
                r.Capacity,
                r.PricePerNight,
                r.Photos.OrderBy(ph => ph.DisplayOrder).FirstOrDefault()?.Url)),
        p.Photos
            .Where(ph => ph.PropertyId == p.Id)
            .OrderBy(ph => ph.DisplayOrder)
            .Select(ph => new PhotoDto(ph.Id, ph.Url, ph.PublicId, ph.IsCover, ph.DisplayOrder)),
        p.CreatedAt);
}
