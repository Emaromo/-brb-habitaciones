namespace BrbHabitaciones.Application.DTOs.Properties;

public record UpdatePropertyRequest(
    string? Name,
    string? Description,
    string? Province,
    string? City,
    string? Address,
    string? PropertyType,
    bool? IsActive);
