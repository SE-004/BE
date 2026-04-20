using BlogApi.Models;
using BlogApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<UserService> _logger;
    private readonly IMetricsService _metrics;

    public UserService(ApplicationDbContext db, ILogger<UserService> logger, IMetricsService metrics)
    {
        _db = db;
        _logger = logger;
        _metrics = metrics;
    }

    public async Task<User?> GetAsync(Guid id)
    {
        _logger.LogInformation("Retrieving user with ID: {UserId}", id);
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
        _metrics.RecordUserCreated();
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