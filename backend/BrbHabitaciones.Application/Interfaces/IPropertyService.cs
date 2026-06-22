using BrbHabitaciones.Application.DTOs.Common;
using BrbHabitaciones.Application.DTOs.Properties;

namespace BrbHabitaciones.Application.Interfaces;

public interface IPropertyService
{
    Task<PagedResult<PropertySummaryDto>> SearchAsync(PropertySearchQuery query);
    Task<PropertyDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<PropertySummaryDto>> GetByOwnerAsync(Guid ownerId);
    Task<PropertyDto> CreateAsync(CreatePropertyRequest request, Guid ownerId);
    Task<PropertyDto> UpdateAsync(Guid id, UpdatePropertyRequest request, Guid requesterId, bool isAdmin = false);
    Task DeleteAsync(Guid id, Guid requesterId, bool isAdmin = false);
}
