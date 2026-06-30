using BrbHabitaciones.Application.DTOs.Common;
using BrbHabitaciones.Domain.Enums;
using BrbHabitaciones.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BrbHabitaciones.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class HealthController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(ApiResponse<object>.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

    // Returns the email and role from the current JWT — use to verify role after login.
    [HttpGet("whoami")]
    [Authorize]
    public IActionResult WhoAmI()
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                 ?? User.FindFirst("email")?.Value;
        var role  = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
                 ?? User.FindFirst("role")?.Value;
        var id    = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Ok(ApiResponse<object>.Ok(new { id, email, role }));
    }

    // Bootstrap endpoint — promotes a user to Administrador.
    // Protected by Bootstrap:Secret env var. Remove after initial setup.
    [HttpPost("make-admin")]
    public async Task<IActionResult> MakeAdmin(
        [FromBody] MakeAdminRequest request,
        [FromServices] IConfiguration config)
    {
        var secret = config["Bootstrap:Secret"] ?? "brb-bootstrap-2026";
        if (request.Secret != secret)
            return Unauthorized(ApiResponse<object>.Fail("Clave incorrecta."));

        var user = await db.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant());

        if (user is null)
            return NotFound(ApiResponse<object>.Fail($"Usuario {request.Email} no encontrado."));

        user.Role = UserRole.Administrador;
        await db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok(new { email = user.Email, role = "Administrador" },
            $"{user.Email} ahora es Administrador."));
    }

    [HttpGet("db")]
    public async Task<IActionResult> DbCheck()
    {
        try
        {
            // Check that Users table exists and Role column type
            var roleColumnType = await db.Database
                .SqlQueryRaw<string>(
                    "SELECT data_type FROM information_schema.columns " +
                    "WHERE table_name = 'Users' AND column_name = 'Role'")
                .FirstOrDefaultAsync();

            var userCount = await db.Users.CountAsync();
            var amenityCount = await db.Amenities.CountAsync();

            return Ok(ApiResponse<object>.Ok(new
            {
                connected = true,
                roleColumnType = roleColumnType ?? "NOT FOUND",
                userCount,
                amenityCount,
                timestamp = DateTime.UtcNow
            }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(
                $"DB error [{ex.GetType().Name}]: {ex.Message} | Inner: {ex.InnerException?.Message}"));
        }
    }
}

public record MakeAdminRequest(string Email, string Secret);
