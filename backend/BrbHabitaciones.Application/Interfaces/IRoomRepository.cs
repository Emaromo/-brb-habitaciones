using BrbHabitaciones.Domain.Entities;

namespace BrbHabitaciones.Application.Interfaces;

public interface IRoomRepository
{
    Task<IEnumerable<Room>> GetByPropertyAsync(Guid propertyId);
    Task<Room?> GetByIdAsync(Guid id);
    Task<bool> IsPropertyOwnerAsync(Guid propertyId, Guid userId);
    Task<Room> AddAsync(Room room);
    Task SaveChangesAsync();
}
