using System.Security.Claims;
using BrbHabitaciones.Application.DTOs.Common;
using BrbHabitaciones.Application.DTOs.Photos;
using BrbHabitaciones.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrbHabitaciones.Api.Controllers;

[ApiController]
[Authorize(Roles = "DuenoAlojamiento,Administrador")]
public class PhotosController(IPhotoService photoService) : ControllerBase
{
    // POST /api/v1/rooms/{roomId}/photos
    [HttpPost("api/v1/rooms/{roomId:guid}/photos")]
    [RequestSizeLimit(10_485_760)] // 10 MB
    public async Task<IActionResult> UploadToRoom(Guid roomId, IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(ApiResponse<object>.Fail("No se recibió ningún archivo."));

        if (!IsValidImage(file.ContentType))
            return BadRequest(ApiResponse<object>.Fail("Solo se permiten imágenes (jpg, png, webp)."));

        var requesterId = GetCurrentUserId();
        await using var stream = file.OpenReadStream();
        var photo = await photoService.UploadToRoomAsync(stream, file.FileName, roomId, requesterId);
        return Ok(ApiResponse<PhotoDto>.Ok(photo, "Foto subida exitosamente."));
    }

    // POST /api/v1/properties/{propertyId}/photos
    [HttpPost("api/v1/properties/{propertyId:guid}/photos")]
    [RequestSizeLimit(10_485_760)]
    public async Task<IActionResult> UploadToProperty(Guid propertyId, IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(ApiResponse<object>.Fail("No se recibió ningún archivo."));

        if (!IsValidImage(file.ContentType))
            return BadRequest(ApiResponse<object>.Fail("Solo se permiten imágenes (jpg, png, webp)."));

        var requesterId = GetCurrentUserId();
        await using var stream = file.OpenReadStream();
        var photo = await photoService.UploadToPropertyAsync(stream, file.FileName, propertyId, requesterId);
        return Ok(ApiResponse<PhotoDto>.Ok(photo, "Foto subida exitosamente."));
    }

    // DELETE /api/v1/photos/{id}
    [HttpDelete("api/v1/photos/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var requesterId = GetCurrentUserId();
        await photoService.DeleteAsync(id, requesterId);
        return Ok(ApiResponse<object>.Ok(null!, "Foto eliminada."));
    }

    private Guid GetCurrentUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private static bool IsValidImage(string contentType) =>
        contentType is "image/jpeg" or "image/jpg" or "image/png" or "image/webp";
}
