namespace BudgetApi.Dtos.Auth;

public record MeResponseDto(
  Guid Id,
  string Email,
  string Name
);