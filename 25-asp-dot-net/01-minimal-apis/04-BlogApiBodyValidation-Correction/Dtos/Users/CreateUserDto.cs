using System.ComponentModel.DataAnnotations;

namespace BlogApi.Dtos.Users;

public record CreateUserDto(
    [property: Required]
    [property: StringLength(100, MinimumLength = 1)]
    string Name,

    [property: Required]
    [property: EmailAddress]
    string Email
);