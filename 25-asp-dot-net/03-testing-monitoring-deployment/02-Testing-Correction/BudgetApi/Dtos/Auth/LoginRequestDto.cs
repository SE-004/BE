using System.ComponentModel.DataAnnotations;

namespace BudgetApi.Dtos.Auth;

public record LoginRequestDto(
  [Required, EmailAddress]
  string Email,

  [Required]
string Password
);