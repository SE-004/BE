using System.ComponentModel.DataAnnotations;

namespace BlogApi.Dtos.Posts;

public record UpdatePostDto(
    [property: StringLength(100, MinimumLength = 1)]
    string? Title,

    [property: StringLength(10_000, MinimumLength = 1)]
    string? Content
);