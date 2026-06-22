using BrbHabitaciones.Application.Interfaces;
using BrbHabitaciones.Application.Services;
using BrbHabitaciones.Infrastructure.Data;
using BrbHabitaciones.Infrastructure.Repositories;
using BrbHabitaciones.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BrbHabitaciones.Infrastructure.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("DefaultConnection")));

        // Auth
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IAuthService, AuthService>();

        // Properties & Rooms
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IPropertyService, PropertyService>();
        services.AddScoped<IRoomService, RoomService>();

        // Photos
        services.AddScoped<IPhotoService, CloudinaryPhotoService>();

        // Reservations
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IReservationService, ReservationService>();

        // Admin
        services.AddScoped<IAdminService, AdminService>();

        return services;
    }
}
