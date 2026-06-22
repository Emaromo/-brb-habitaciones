using BrbHabitaciones.Application.DTOs.Amenities;
using BrbHabitaciones.Application.DTOs.Photos;
using BrbHabitaciones.Application.DTOs.Rooms;
using BrbHabitaciones.Application.Interfaces;
using BrbHabitaciones.Domain.Entities;

namespace BrbHabitaciones.Application.Services;

public class RoomService(IRoomRepository repo) : IRoomService
{
    public async Task<IEnumerable<RoomDto>> GetByPropertyAsync(Guid propertyId)
    {
        var rooms = await repo.GetByPropertyAsync(propertyId);
        return rooms.Select(MapToDto);
    }

    public async Task<RoomDto?> GetByIdAsync(Guid id)
    {
        var room = await repo.GetByIdAsync(id);
        return room is null ? null : MapToDto(room);
    }

    public async Task<RoomDto> CreateAsync(Guid propertyId, CreateRoomRequest request, Guid ownerId)
    {
        var isOwner = await repo.IsPropertyOwnerAsync(propertyId, ownerId);
        if (!isOwner)
            throw new UnauthorizedAccessException("No tenés permiso para agregar habitaciones a esta propiedad.");

        var room = new Room
        {
            PropertyId = propertyId,
            Title = request.Title,
            Description = request.Description,
            Capacity = request.Capacity,
            PricePerNight = request.PricePerNight,
        };

        var created = await repo.AddAsync(room);
        await repo.SaveChangesAsync();

        var full = await repo.GetByIdAsync(created.Id)
            ?? throw new InvalidOperationException("Error al recuperar la habitación creada.");
        return MapToDto(full);
    }

    public async Task<RoomDto> UpdateAsync(Guid id, UpdateRoomRequest request, Guid requesterId, bool isAdmin = false)
    {
        var room = await repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Habitación no encontrada.");

        var isOwner = await repo.IsPropertyOwnerAsync(room.PropertyId, requesterId);
        if (!isOwner && !isAdmin)
            throw new UnauthorizedAccessException("No tenés permiso para modificar esta habitación.");

        if (request.Title is not null) room.Title = request.Title;
        if (request.Description is not null) room.Description = request.Description;
        if (request.Capacity.HasValue) room.Capacity = request.Capacity.Value;
        if (request.PricePerNight.HasValue) room.PricePerNight = request.PricePerNight.Value;
        if (request.IsActive.HasValue) room.IsActive = request.IsActive.Value;

        room.UpdatedAt = DateTime.UtcNow;
        await repo.SaveChangesAsync();

        var updated = await repo.GetByIdAsync(id);
        return MapToDto(updated!);
    }

    public async Task DeleteAsync(Guid id, Guid requesterId, bool isAdmin = false)
    {
        var room = await repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Habitación no encontrada.");

        var isOwner = await repo.IsPropertyOwnerAsync(room.PropertyId, requesterId);
        if (!isOwner && !isAdmin)
            throw new UnauthorizedAccessException("No tenés permiso para eliminar esta habitación.");

        room.DeletedAt = DateTime.UtcNow;
        room.IsActive = false;
        await repo.SaveChangesAsync();
    }

    public async Task<RoomDto> AddAmenityAsync(Guid roomId, Guid amenityId, Guid requesterId)
    {
        var room = await repo.GetByIdAsync(roomId)
            ?? throw new KeyNotFoundException("Habitación no encontrada.");

        var isOwner = await repo.IsPropertyOwnerAsync(room.PropertyId, requesterId);
        if (!isOwner)
            throw new UnauthorizedAccessException("No tenés permiso para modificar esta habitación.");

        if (room.RoomAmenities.Any(ra => ra.AmenityId == amenityId))
            return MapToDto(room);

        room.RoomAmenities.Add(new RoomAmenity { RoomId = roomId, AmenityId = amenityId });
        await repo.SaveChangesAsync();

        var updated = await repo.GetByIdAsync(roomId);
        return MapToDto(updated!);
    }

    public async Task RemoveAmenityAsync(Guid roomId, Guid amenityId, Guid requesterId)
    {
        var room = await repo.GetByIdAsync(roomId)
            ?? throw new KeyNotFoundException("Habitación no encontrada.");

        var isOwner = await repo.IsPropertyOwnerAsync(room.PropertyId, requesterId);
        if (!isOwner)
            throw new UnauthorizedAccessException("No tenés permiso para modificar esta habitación.");

        var link = room.RoomAmenities.FirstOrDefault(ra => ra.AmenityId == amenityId);
        if (link is not null)
        {
            room.RoomAmenities.Remove(link);
            await repo.SaveChangesAsync();
        }
    }

    private static RoomDto MapToDto(Room r) => new(
        r.Id,
        r.PropertyId,
        r.Property.Name,
        r.Title,
        r.Description,
        r.Capacity,
        r.PricePerNight,
        r.IsActive,
        r.Photos
            .OrderBy(p => p.DisplayOrder)
            .Select(p => new PhotoDto(p.Id, p.Url, p.PublicId, p.IsCover, p.DisplayOrder)),
        r.RoomAmenities
            .Select(ra => new AmenityDto(ra.Amenity.Id, ra.Amenity.Name, ra.Amenity.Icon, ra.Amenity.Category)),
        r.CreatedAt);
}
