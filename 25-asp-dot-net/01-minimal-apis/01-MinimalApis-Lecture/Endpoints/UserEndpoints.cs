using BlogApi.DTOs;
namespace BlogApi.Endpoints;

public static class UserEndpoints
{
  public static RouteGroupBuilder MapUsers(this IEndpointRouteBuilder routes)
  {
    var group = routes.MapGroup("/users");
    group.MapGet("/", () => new[] { "Alice", "Bob" });
    group.MapPost("/", (UserRequestDto user) => $"Created {user.Name}");
    group.MapGet("/{id:int}", (int id) => $"User {id}");
    group.MapGet("/info", () => "User info");
    return group;
  }
}