namespace BudgetApi.Dtos.Auth;

public record AuthResponseDto(
  string Token,
  DateTime ExpiresAtUtc
);