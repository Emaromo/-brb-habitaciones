using BrbHabitaciones.Application.DTOs.Admin;
using BrbHabitaciones.Application.DTOs.Reservations;
using BrbHabitaciones.Application.Interfaces;
using BrbHabitaciones.Domain.Enums;
using BrbHabitaciones.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BrbHabitaciones.Infrastructure.Services;

public class AdminService(AppDbContext db) : IAdminService
{
    public async Task<AdminStatsDto> GetStatsAsync()
    {
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var totalUsers = await db.Users.CountAsync(u => u.DeletedAt == null);
        var totalProperties = await db.Properties.CountAsync(p => p.DeletedAt == null);
        var totalRooms = await db.Rooms.CountAsync(r => r.DeletedAt == null);
        var pendingProperties = await db.Properties.CountAsync(p => !p.IsApproved && p.DeletedAt == null);

        var reservationsThisMonth = await db.Reservations
            .CountAsync(r => r.CreatedAt >= monthStart && r.Status != ReservationStatus.Cancelada);

        var revenueThisMonth = await db.Reservations
            .Where(r => r.CreatedAt >= monthStart && r.Status == ReservationStatus.Confirmada)
            .SumAsync(r => (decimal?)r.TotalPrice) ?? 0m;

        var activeReservations = await db.Reservations
            .CountAsync(r => r.Status == ReservationStatus.Confirmada);

        return new AdminStatsDto(
            totalUsers,
            totalProperties,
            totalRooms,
            pendingProperties,
            reservationsThisMonth,
            revenueThisMonth,
            activeReservations);
    }

    public async Task<IEnumerable<AdminUserDto>> GetUsersAsync() =>
        await db.Users
            .Where(u => u.DeletedAt == null)
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new AdminUserDto(u.Id, u.Email, u.FirstName, u.LastName,
                u.Role.ToString(), u.IsActive, u.CreatedAt))
            .ToListAsync();

    public async Task<AdminUserDto> ChangeUserRoleAsync(Guid userId, string newRole)
    {
        if (!Enum.TryParse<UserRole>(newRole, out var parsed))
            throw new InvalidOperationException($"Rol inválido: {newRole}. Valores válidos: Cliente, DuenoAlojamiento, Administrador.");

        var user = await db.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException("Usuario no encontrado.");

        user.Role = parsed;
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return new AdminUserDto(user.Id, user.Email, user.FirstName, user.LastName,
            user.Role.ToString(), user.IsActive, user.CreatedAt);
    }

    public async Task<AdminUserDto> ToggleUserActiveAsync(Guid userId)
    {
        var user = await db.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException("Usuario no encontrado.");

        user.IsActive = !user.IsActive;
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return new AdminUserDto(user.Id, user.Email, user.FirstName, user.LastName,
            user.Role.ToString(), user.IsActive, user.CreatedAt);
    }

    public async Task<IEnumerable<AdminPropertyDto>> GetPropertiesAsync()
    {
        var properties = await db.Properties
            .Include(p => p.Owner)
            .Include(p => p.Rooms)
            .Where(p => p.DeletedAt == null)
            .OrderBy(p => p.IsApproved)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync();

        return properties.Select(p => new AdminPropertyDto(
            p.Id,
            p.Name,
            $"{p.Owner.FirstName} {p.Owner.LastName}",
            p.Province,
            p.City,
            p.PropertyType.ToString(),
            p.IsActive,
            p.IsApproved,
            p.Rooms.Count(r => r.DeletedAt == null),
            p.CreatedAt));
    }

    public async Task<AdminPropertyDto> SetPropertyApprovalAsync(Guid propertyId, bool approved)
    {
        var property = await db.Properties
            .Include(p => p.Owner)
            .Include(p => p.Rooms)
            .FirstOrDefaultAsync(p => p.Id == propertyId)
            ?? throw new KeyNotFoundException("Propiedad no encontrada.");

        property.IsApproved = approved;
        property.IsActive = approved;
        property.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return new AdminPropertyDto(
            property.Id,
            property.Name,
            $"{property.Owner.FirstName} {property.Owner.LastName}",
            property.Province,
            property.City,
            property.PropertyType.ToString(),
            property.IsActive,
            property.IsApproved,
            property.Rooms.Count(r => r.DeletedAt == null),
            property.CreatedAt);
    }

    public async Task<IEnumerable<ReservationDto>> GetAllReservationsAsync()
    {
        var reservations = await db.Reservations
            .Include(r => r.Room).ThenInclude(room => room.Property)
            .Include(r => r.Client)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return reservations.Select(r => new ReservationDto(
            r.Id, r.RoomId, r.Room.Title, r.Room.PropertyId,
            r.Room.Property.Name, r.Room.Property.City,
            r.ClientId, $"{r.Client.FirstName} {r.Client.LastName}",
            r.CheckInDate, r.CheckOutDate, r.GuestCount,
            r.CheckOutDate.DayNumber - r.CheckInDate.DayNumber,
            r.TotalPrice, r.Status.ToString(), r.CancellationReason, r.CreatedAt));
    }
}
