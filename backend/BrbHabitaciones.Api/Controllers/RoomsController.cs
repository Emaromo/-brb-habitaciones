using System.Security.Claims;
using BrbHabitaciones.Application.DTOs.Common;
using BrbHabitaciones.Application.DTOs.Rooms;
using BrbHabitaciones.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrbHabitaciones.Api.Controllers;

[ApiController]
public class RoomsController(IRoomService roomService) : ControllerBase
{
    // GET /api/v1/properties/{propertyId}/rooms
    [HttpGet("api/v1/properties/{propertyId:guid}/rooms")]
    public async Task<IActionResult> GetByProperty(Guid propertyId)
    {
        var rooms = await roomService.GetByPropertyAsync(propertyId);
        return Ok(ApiResponse<IEnumerable<RoomDto>>.Ok(rooms));
    }

    // GET /api/v1/rooms/{id}
    [HttpGet("api/v1/rooms/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var room = await roomService.GetByIdAsync(id);
        if (room is null)
            return NotFound(ApiResponse<object>.Fail("Habitación no encontrada."));
        return Ok(ApiResponse<RoomDto>.Ok(room));
    }

    // POST /api/v1/properties/{propertyId}/rooms
    [HttpPost("api/v1/properties/{propertyId:guid}/rooms")]
    [Authorize(Roles = "DuenoAlojamiento,Administrador")]
    public async Task<IActionResult> Create(Guid propertyId, [FromBody] CreateRoomRequest request)
    {
        var ownerId = GetCurrentUserId();
        var created = await roomService.CreateAsync(propertyId, request, ownerId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<RoomDto>.Ok(created, "Habitación creada exitosamente."));
    }

    // PUT /api/v1/rooms/{id}
    [HttpPut("api/v1/rooms/{id:guid}")]
    [Authorize(Roles = "DuenoAlojamiento,Administrador")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoomRequest request)
    {
        var requesterId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Administrador");
        var updated = await roomService.UpdateAsync(id, request, requesterId, isAdmin);
        return Ok(ApiResponse<RoomDto>.Ok(updated));
    }

    // DELETE /api/v1/rooms/{id}
    [HttpDelete("api/v1/rooms/{id:guid}")]
    [Authorize(Roles = "DuenoAlojamiento,Administrador")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var requesterId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Administrador");
        await roomService.DeleteAsync(id, requesterId, isAdmin);
        return Ok(ApiResponse<object>.Ok(null!, "Habitación eliminada."));
    }

    // POST /api/v1/rooms/{id}/amenities/{amenityId}
    [HttpPost("api/v1/rooms/{id:guid}/amenities/{amenityId:guid}")]
    [Authorize(Roles = "DuenoAlojamiento,Administrador")]
    public async Task<IActionResult> AddAmenity(Guid id, Guid amenityId)
    {
        var requesterId = GetCurrentUserId();
        var updated = await roomService.AddAmenityAsync(id, amenityId, requesterId);
        return Ok(ApiResponse<RoomDto>.Ok(updated));
    }

    // DELETE /api/v1/rooms/{id}/amenities/{amenityId}
    [HttpDelete("api/v1/rooms/{id:guid}/amenities/{amenityId:guid}")]
    [Authorize(Roles = "DuenoAlojamiento,Administrador")]
    public async Task<IActionResult> RemoveAmenity(Guid id, Guid amenityId)
    {
        var requesterId = GetCurrentUserId();
        await roomService.RemoveAmenityAsync(id, amenityId, requesterId);
        return Ok(ApiResponse<object>.Ok(null!, "Amenity removido."));
    }

    private Guid GetCurrentUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
