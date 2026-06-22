using System.Security.Claims;
using BrbHabitaciones.Application.DTOs.Common;
using BrbHabitaciones.Application.DTOs.Reservations;
using BrbHabitaciones.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrbHabitaciones.Api.Controllers;

[ApiController]
[Route("api/v1")]
public class ReservationsController(IReservationService reservationService) : ControllerBase
{
    // GET /api/v1/rooms/{roomId}/availability?from=2026-07-01&to=2026-07-31
    [HttpGet("rooms/{roomId:guid}/availability")]
    public async Task<IActionResult> GetAvailability(
        Guid roomId,
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to)
    {
        if (to <= from)
            return BadRequest(ApiResponse<object>.Fail("La fecha 'to' debe ser posterior a 'from'."));

        var bookedDates = await reservationService.GetBookedDatesAsync(roomId, from, to);
        return Ok(ApiResponse<IEnumerable<DateOnly>>.Ok(bookedDates));
    }

    // POST /api/v1/reservations
    [HttpPost("reservations")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateReservationRequest request)
    {
        var clientId = GetCurrentUserId();
        var reservation = await reservationService.CreateAsync(request, clientId);
        return CreatedAtAction(nameof(GetById), new { id = reservation.Id },
            ApiResponse<ReservationDto>.Ok(reservation, "Reserva confirmada exitosamente."));
    }

    // GET /api/v1/reservations/my
    [HttpGet("reservations/my")]
    [Authorize]
    public async Task<IActionResult> GetMy()
    {
        var userId = GetCurrentUserId();
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "Cliente";
        var reservations = await reservationService.GetMyAsync(userId, role);
        return Ok(ApiResponse<IEnumerable<ReservationDto>>.Ok(reservations));
    }

    // GET /api/v1/reservations/{id}
    [HttpGet("reservations/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var reservation = await reservationService.GetByIdAsync(id);
        if (reservation is null)
            return NotFound(ApiResponse<object>.Fail("Reserva no encontrada."));
        return Ok(ApiResponse<ReservationDto>.Ok(reservation));
    }

    // PUT /api/v1/reservations/{id}/cancel
    [HttpPut("reservations/{id:guid}/cancel")]
    [Authorize]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelReservationRequest? request)
    {
        var requesterId = GetCurrentUserId();
        var reservation = await reservationService.CancelAsync(id, requesterId, request?.Reason);
        return Ok(ApiResponse<ReservationDto>.Ok(reservation, "Reserva cancelada."));
    }

    private Guid GetCurrentUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

public record CancelReservationRequest(string? Reason);
