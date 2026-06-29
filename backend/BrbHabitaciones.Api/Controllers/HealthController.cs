using BrbHabitaciones.Application.DTOs.Common;
using BrbHabitaciones.Infrastructure.Data;
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

    // Diagnostic endpoint — returns DB schema info to identify column type mismatches.
    // Remove once the production DB issue is resolved.
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
