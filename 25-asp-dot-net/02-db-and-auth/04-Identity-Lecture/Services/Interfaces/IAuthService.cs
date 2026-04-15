using BlogApi.Dtos.Auth;

namespace BlogApi.Services;

public interface IAuthService
{
  Task<(bool Success, IEnumerable<object> Errors)> RegisterAsync(RegisterRequestDto req);
  Task<AuthResponseDto?> LoginAsync(LoginRequestDto req);
  Task<object?> GetCurrentUserAsync(string UserId);
}