using BrbHabitaciones.Application.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace BrbHabitaciones.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(ApiResponse<object>.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));
}
