using BrbHabitaciones.Application.DTOs.Admin;
using BrbHabitaciones.Application.DTOs.Common;
using BrbHabitaciones.Application.DTOs.Reservations;
using BrbHabitaciones.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrbHabitaciones.Api.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Administrador")]
public class AdminController(IAdminService adminService) : ControllerBase
{
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await adminService.GetStatsAsync();
        return Ok(ApiResponse<AdminStatsDto>.Ok(stats));
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await adminService.GetUsersAsync();
        return Ok(ApiResponse<IEnumerable<AdminUserDto>>.Ok(users));
    }

    [HttpPut("users/{id:guid}/role")]
    public async Task<IActionResult> ChangeUserRole(Guid id, [FromBody] ChangeRoleRequest request)
    {
        var user = await adminService.ChangeUserRoleAsync(id, request.Role);
        return Ok(ApiResponse<AdminUserDto>.Ok(user, "Rol actualizado."));
    }

    [HttpPut("users/{id:guid}/deactivate")]
    public async Task<IActionResult> ToggleUserActive(Guid id)
    {
        var user = await adminService.ToggleUserActiveAsync(id);
        var msg = user.IsActive ? "Usuario reactivado." : "Usuario desactivado.";
        return Ok(ApiResponse<AdminUserDto>.Ok(user, msg));
    }

    [HttpGet("properties")]
    public async Task<IActionResult> GetProperties()
    {
        var properties = await adminService.GetPropertiesAsync();
        return Ok(ApiResponse<IEnumerable<AdminPropertyDto>>.Ok(properties));
    }

    [HttpPut("properties/{id:guid}/approve")]
    public async Task<IActionResult> ApproveProperty(Guid id)
    {
        var property = await adminService.SetPropertyApprovalAsync(id, true);
        return Ok(ApiResponse<AdminPropertyDto>.Ok(property, "Propiedad aprobada."));
    }

    [HttpPut("properties/{id:guid}/reject")]
    public async Task<IActionResult> RejectProperty(Guid id)
    {
        var property = await adminService.SetPropertyApprovalAsync(id, false);
        return Ok(ApiResponse<AdminPropertyDto>.Ok(property, "Propiedad rechazada."));
    }

    [HttpGet("reservations")]
    public async Task<IActionResult> GetReservations()
    {
        var reservations = await adminService.GetAllReservationsAsync();
        return Ok(ApiResponse<IEnumerable<ReservationDto>>.Ok(reservations));
    }
}

public record ChangeRoleRequest(string Role);
