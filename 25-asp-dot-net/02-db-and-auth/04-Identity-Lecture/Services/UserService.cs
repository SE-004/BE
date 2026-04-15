using BlogApi.Models;
using BlogApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _db;

    public UserService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetAsync(Guid id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task<IReadOnlyList<User>> ListAsync()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task<User> CreateAsync(string name, string email)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<User?> UpdateAsync(Guid id, string? name, string? email)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return null;

        if (name != null)
            user.Name = name;

        if (email != null)
            user.Email = email;

        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return false;

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return true;
    }
}