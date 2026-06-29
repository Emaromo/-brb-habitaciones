using BrbHabitaciones.Application.DTOs.Auth;
using BrbHabitaciones.Application.DTOs.Common;
using BrbHabitaciones.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BrbHabitaciones.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        logger.LogInformation("[AUTH] Register START — email={Email}", request.Email);
        var result = await authService.RegisterAsync(request);
        logger.LogInformation("[AUTH] Register OK — userId={UserId}", result.User.Id);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<AuthResponse>.Ok(result, "Cuenta creada exitosamente."));
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        logger.LogInformation("[AUTH] Login START — email={Email}", request.Email);
        var result = await authService.LoginAsync(request);
        logger.LogInformation("[AUTH] Login OK — userId={UserId}", result.User.Id);
        return Ok(ApiResponse<AuthResponse>.Ok(result));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        var result = await authService.RefreshTokenAsync(request.RefreshToken);
        return Ok(ApiResponse<AuthResponse>.Ok(result));
    }

    [HttpPost("revoke")]
    [Authorize]
    public async Task<IActionResult> Revoke([FromBody] RefreshRequest request)
    {
        await authService.RevokeTokenAsync(request.RefreshToken);
        return Ok(ApiResponse<object>.Ok(null!, "Sesión cerrada."));
    }
}

public record RefreshRequest(string RefreshToken);
