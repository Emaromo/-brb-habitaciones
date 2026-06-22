namespace BrbHabitaciones.Application.DTOs.Properties;

public record CreatePropertyRequest(
    string Name,
    string Description,
    string Province,
    string City,
    string Address,
    string PropertyType);
