using BrbHabitaciones.Domain.Entities;

namespace BrbHabitaciones.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    DateTime GetRefreshTokenExpiry();
}
