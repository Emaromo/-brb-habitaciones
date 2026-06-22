using BrbHabitaciones.Application.DTOs.Properties;
using BrbHabitaciones.Domain.Entities;

namespace BrbHabitaciones.Application.Interfaces;

public interface IPropertyRepository
{
    Task<(IEnumerable<Property> Items, int TotalCount)> SearchAsync(PropertySearchQuery query);
    Task<Property?> GetByIdAsync(Guid id);
    Task<IEnumerable<Property>> GetByOwnerAsync(Guid ownerId);
    Task<Property> AddAsync(Property property);
    Task SaveChangesAsync();
}
