using BrbHabitaciones.Application.DTOs.Auth;
using BrbHabitaciones.Application.Interfaces;
using BrbHabitaciones.Domain.Entities;
using BrbHabitaciones.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace BrbHabitaciones.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IJwtService jwtService,
    IPasswordHasher passwordHasher,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        logger.LogInformation("[AuthService] RegisterAsync — step 1: checking email uniqueness for {Email}", request.Email);

        if (await userRepository.ExistsByEmailAsync(request.Email.ToLowerInvariant()))
            throw new InvalidOperationException("Ya existe una cuenta con ese email.");

        logger.LogInformation("[AuthService] RegisterAsync — step 2: hashing password");
        var hash = passwordHasher.Hash(request.Password);

        var user = new User
        {
            Email = request.Email.ToLowerInvariant(),
            PasswordHash = hash,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            Role = UserRole.Cliente
        };

        logger.LogInformation("[AuthService] RegisterAsync — step 3: saving user to DB (userId={UserId})", user.Id);
        await userRepository.CreateAsync(user);

        logger.LogInformation("[AuthService] RegisterAsync — step 4: user saved OK, building auth response");
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
        logger.LogInformation("[AuthService] BuildAuthResponse — step A: generating JWT for userId={UserId}", user.Id);
        var accessToken = jwtService.GenerateAccessToken(user);

        logger.LogInformation("[AuthService] BuildAuthResponse — step B: generating refresh token");
        var refreshTokenValue = jwtService.GenerateRefreshToken();
        var expiry = jwtService.GetRefreshTokenExpiry();

        logger.LogInformation("[AuthService] BuildAuthResponse — step C: saving refresh token to DB");
        await userRepository.AddRefreshTokenAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = expiry
        });
        await userRepository.SaveChangesAsync();

        logger.LogInformation("[AuthService] BuildAuthResponse — step D: all done, returning AuthResponse");
        return new AuthResponse(
            accessToken,
            refreshTokenValue,
            expiry,
            new UserDto(user.Id, user.Email, user.FirstName, user.LastName, user.Role.ToString())
        );
    }
}
