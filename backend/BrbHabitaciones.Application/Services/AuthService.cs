using BrbHabitaciones.Application.DTOs.Auth;
using BrbHabitaciones.Application.Interfaces;
using BrbHabitaciones.Domain.Entities;
using BrbHabitaciones.Domain.Enums;

namespace BrbHabitaciones.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IJwtService jwtService,
    IPasswordHasher passwordHasher) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await userRepository.ExistsByEmailAsync(request.Email.ToLowerInvariant()))
            throw new InvalidOperationException("Ya existe una cuenta con ese email.");

        var user = new User
        {
            Email = request.Email.ToLowerInvariant(),
            PasswordHash = passwordHasher.Hash(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            Role = UserRole.Cliente
        };

        await userRepository.CreateAsync(user);
        return await BuildAuthResponseAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email.ToLowerInvariant())
            ?? throw new UnauthorizedAccessException("Credenciales incorrectas.");

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales incorrectas.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Tu cuenta está desactivada.");

        return await BuildAuthResponseAsync(user);
    }

    public async Task<AuthResponse> RefreshTokenAsync(string token)
    {
        var refreshToken = await userRepository.GetRefreshTokenAsync(token)
            ?? throw new UnauthorizedAccessException("Token inválido.");

        if (!refreshToken.IsActive)
            throw new UnauthorizedAccessException("Token expirado o revocado.");

        refreshToken.RevokedAt = DateTime.UtcNow;
        return await BuildAuthResponseAsync(refreshToken.User);
    }

    public async Task RevokeTokenAsync(string token)
    {
        var refreshToken = await userRepository.GetRefreshTokenAsync(token)
            ?? throw new UnauthorizedAccessException("Token inválido.");

        refreshToken.RevokedAt = DateTime.UtcNow;
        await userRepository.SaveChangesAsync();
    }

    private async Task<AuthResponse> BuildAuthResponseAsync(User user)
    {
        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshTokenValue = jwtService.GenerateRefreshToken();
        var expiry = jwtService.GetRefreshTokenExpiry();

        await userRepository.AddRefreshTokenAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = expiry
        });
        await userRepository.SaveChangesAsync();

        return new AuthResponse(
            accessToken,
            refreshTokenValue,
            expiry,
            new UserDto(user.Id, user.Email, user.FirstName, user.LastName, user.Role.ToString())
        );
    }
}
