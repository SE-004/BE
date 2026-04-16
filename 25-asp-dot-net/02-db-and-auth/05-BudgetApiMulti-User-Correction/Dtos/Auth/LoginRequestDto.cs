using System.ComponentModel.DataAnnotations;

namespace BudgetApi.Dtos.Auth;

public record LoginRequestDto(
  [Required, EmailAddress]
  string Email,

  [Required]
string Password
);

// public class LoginRequestDto
// {
//   [Required, EmailAddress]
//   public string Email { get; set; } = string.Empty;

//   [Required]
//   public string Password { get; set; } = string.Empty;
// }