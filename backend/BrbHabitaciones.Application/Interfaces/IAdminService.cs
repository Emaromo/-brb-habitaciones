using BrbHabitaciones.Application.DTOs.Admin;
using BrbHabitaciones.Application.DTOs.Reservations;

namespace BrbHabitaciones.Application.Interfaces;

public interface IAdminService
{
    Task<AdminStatsDto> GetStatsAsync();

    Task<IEnumerable<AdminUserDto>> GetUsersAsync();
    Task<AdminUserDto> ChangeUserRoleAsync(Guid userId, string newRole);
    Task<AdminUserDto> ToggleUserActiveAsync(Guid userId);

    Task<IEnumerable<AdminPropertyDto>> GetPropertiesAsync();
    Task<AdminPropertyDto> SetPropertyApprovalAsync(Guid propertyId, bool approved);

    Task<IEnumerable<ReservationDto>> GetAllReservationsAsync();
}
