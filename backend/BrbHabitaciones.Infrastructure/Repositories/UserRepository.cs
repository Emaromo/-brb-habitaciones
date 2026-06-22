using BrbHabitaciones.Application.Interfaces;
using BrbHabitaciones.Domain.Entities;
using BrbHabitaciones.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BrbHabitaciones.Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email) =>
        await db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByIdAsync(Guid id) =>
        await db.Users.FindAsync(id);

    public async Task<User> CreateAsync(User user)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }

    public async Task<bool> ExistsByEmailAsync(string email) =>
        await db.Users.AnyAsync(u => u.Email == email);

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token) =>
        await db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken) =>
        await db.RefreshTokens.AddAsync(refreshToken);

    public async Task SaveChangesAsync() =>
        await db.SaveChangesAsync();
}
