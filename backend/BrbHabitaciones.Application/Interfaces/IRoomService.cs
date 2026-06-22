using BrbHabitaciones.Application.DTOs.Rooms;

namespace BrbHabitaciones.Application.Interfaces;

public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetByPropertyAsync(Guid propertyId);
    Task<RoomDto?> GetByIdAsync(Guid id);
    Task<RoomDto> CreateAsync(Guid propertyId, CreateRoomRequest request, Guid ownerId);
    Task<RoomDto> UpdateAsync(Guid id, UpdateRoomRequest request, Guid requesterId, bool isAdmin = false);
    Task DeleteAsync(Guid id, Guid requesterId, bool isAdmin = false);
    Task<RoomDto> AddAmenityAsync(Guid roomId, Guid amenityId, Guid requesterId);
    Task RemoveAmenityAsync(Guid roomId, Guid amenityId, Guid requesterId);
}
