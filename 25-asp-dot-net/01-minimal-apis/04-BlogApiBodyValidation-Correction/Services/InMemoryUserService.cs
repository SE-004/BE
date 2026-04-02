using BlogApi.Models;
using BlogApi.Services.Interfaces;

namespace BlogApi.Services;

public class InMemoryUserService : IUserService
{

  private readonly Dictionary<Guid, User> _users = new();

  public async Task<User?> GetAsync(Guid id)
  {
    _users.TryGetValue(id, out var user);
    return user;
  }
  public async Task<IReadOnlyList<User>> ListAsync()
  {
    return _users.Values.ToList();
  }
  public async Task<User> CreateAsync(string name, string email)
  {
    var user = new User
    {
      Id = Guid.NewGuid(),
      Name = name,
      Email = email,
      CreatedAt = DateTimeOffset.Now
    };

    _users[user.Id] = user;

    return user;
  }
  public async Task<User?> UpdateAsync(Guid id, string? name, string? email)
  {
    if (!_users.TryGetValue(id, out var user))
      return null;

    if (name is not null)
      user.Name = name;

    if (email is not null)
      user.Email = email;

    return user;
  }
  public async Task<bool> DeleteAsync(Guid id)
  {
    // cascade delete or failure logic in endpoint
    return _users.Remove(id);
  }
}