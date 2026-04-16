using System.ComponentModel.DataAnnotations;

namespace BudgetApi.Dtos.Auth;

public record RegisterRequestDto(
  [Required]
  string Name,

  [Required, EmailAddress]
  string Email,

  [Required, MinLength(6)]
  string Password
);

// public class RegisterRequestDto
// {
//   [Required]
//   public string Name { get; set; } = string.Empty;

//   [Required, EmailAddress]
//   public string Email { get; set; } = string.Empty;

//   [Required, MinLength(6)]
//   public string Password { get; set; } = string.Empty;
// }