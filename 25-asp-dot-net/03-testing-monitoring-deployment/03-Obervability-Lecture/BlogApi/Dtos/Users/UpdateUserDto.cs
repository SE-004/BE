using System.ComponentModel.DataAnnotations;

namespace BlogApi.Dtos.Users;

public record UpdateUserDto(
    [property: StringLength(100, MinimumLength = 1)]
    string? Name,

    [property: EmailAddress]
    string? Email
);