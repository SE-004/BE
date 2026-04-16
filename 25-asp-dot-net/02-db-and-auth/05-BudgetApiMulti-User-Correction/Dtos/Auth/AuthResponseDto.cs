namespace BudgetApi.Dtos.Auth;

public record AuthResponseDto(
  string Token,
  DateTime ExpiresAtUtc
);
// public class AuthResponseDto
// {
//   public string Token { get; set; } = string.Empty;
//   public DateTime ExpiresAtUtc { get; set; }
// }