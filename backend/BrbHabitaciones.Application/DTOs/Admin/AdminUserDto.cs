namespace BrbHabitaciones.Application.DTOs.Admin;

public record AdminUserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    bool IsActive,
    DateTime CreatedAt);
