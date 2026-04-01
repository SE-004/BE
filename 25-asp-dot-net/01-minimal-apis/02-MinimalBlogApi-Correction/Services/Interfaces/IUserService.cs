using BlogApi.Models;

namespace BlogApi.Services.Interfaces;

public interface IUserService
{
  Task<User?> GetAsync(Guid id);
  Task<IReadOnlyList<User>> ListAsync();
  Task<User> CreateAsync(string name, string email);
  Task<User?> UpdateAsync(Guid id, string? name, string? email);
  Task<bool> DeleteAsync(Guid id);
}