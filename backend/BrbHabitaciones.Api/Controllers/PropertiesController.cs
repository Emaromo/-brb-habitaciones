using System.Security.Claims;
using BrbHabitaciones.Application.DTOs.Common;
using BrbHabitaciones.Application.DTOs.Properties;
using BrbHabitaciones.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrbHabitaciones.Api.Controllers;

[ApiController]
[Route("api/v1/properties")]
public class PropertiesController(IPropertyService propertyService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PropertySearchQuery query)
    {
        var result = await propertyService.SearchAsync(query);
        return Ok(ApiResponse<PagedResult<PropertySummaryDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var property = await propertyService.GetByIdAsync(id);
        if (property is null)
            return NotFound(ApiResponse<object>.Fail("Propiedad no encontrada."));
        return Ok(ApiResponse<PropertyDto>.Ok(property));
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMyProperties()
    {
        var ownerId = GetCurrentUserId();
        var items = await propertyService.GetByOwnerAsync(ownerId);
        return Ok(ApiResponse<IEnumerable<PropertySummaryDto>>.Ok(items));
    }

    [HttpPost]
    [Authorize(Roles = "DuenoAlojamiento,Administrador")]
    public async Task<IActionResult> Create([FromBody] CreatePropertyRequest request)
    {
        var ownerId = GetCurrentUserId();
        var created = await propertyService.CreateAsync(request, ownerId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<PropertyDto>.Ok(created, "Propiedad creada exitosamente."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "DuenoAlojamiento,Administrador")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePropertyRequest request)
    {
        var requesterId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Administrador");
        var updated = await propertyService.UpdateAsync(id, request, requesterId, isAdmin);
        return Ok(ApiResponse<PropertyDto>.Ok(updated));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "DuenoAlojamiento,Administrador")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var requesterId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Administrador");
        await propertyService.DeleteAsync(id, requesterId, isAdmin);
        return Ok(ApiResponse<object>.Ok(null!, "Propiedad eliminada."));
    }

    private Guid GetCurrentUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
