using BlogApi.Dtos.Auth;

namespace BlogApi.Services;

public interface IAuthService
{
    Task<(bool Success, IEnumerable<object> Errors)> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request);
    Task<object?> GetCurrentUserAsync(string userId);
}