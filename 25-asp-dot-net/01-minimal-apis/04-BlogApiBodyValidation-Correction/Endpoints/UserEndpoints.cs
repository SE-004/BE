using BlogApi.Dtos.Users;
using BlogApi.Dtos.Posts;
using BlogApi.Services.Interfaces;
using BlogApi.Filters;

namespace BlogApi.Endpoints;

public static class UserEndpoints
{
  public static RouteGroupBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/users");

    // GET /users
    group.MapGet("/", async (IUserService userService) =>
    {
      var users = await userService.ListAsync();
      var userDtos = users.Select(user => new UserResponseDto(user.Id, user.Name, user.Email, user.CreatedAt));

      return Results.Ok(userDtos);
    });
    // POST /users
    group.MapPost("/", async (IUserService userService, CreateUserDto createUserDto, HttpContext context) =>
    {
      var newUser = await userService.CreateAsync(createUserDto.Name, createUserDto.Email);
      var userDto = new UserResponseDto(newUser.Id, newUser.Name, newUser.Email, newUser.CreatedAt);

      var location = $"{context.Request.Scheme}://{context.Request.Host}/users/{newUser.Id}";
      return Results.Created(location, userDto);
    }).WithValidation<CreateUserDto>();

    // GET /users/{id:guid}
    group.MapGet("/{id:guid}", async (Guid id, IUserService userService) =>
    {
      var user = await userService.GetAsync(id);

      if (user is null)
        return Results.NotFound();

      var userDto = new UserResponseDto(user.Id, user.Name, user.Email, user.CreatedAt);

      return Results.Ok(user);
    });

    // PATCH /users/{id:guid}
    group.MapPatch("/{id:guid}", async (Guid id, UpdateUserDto updateUserDto, IUserService userService) =>
    {
      var user = await userService.UpdateAsync(id, updateUserDto.Name, updateUserDto.Email);

      if (user is null)
        return Results.NotFound();

      var userDto = new UserResponseDto(user.Id, user.Name, user.Email, user.CreatedAt);
      return Results.Ok(userDto);
    }).WithValidation<UpdateUserDto>();

    // DELETE /users/{id:guid}
    group.MapDelete("/{id:guid}", async (Guid id, IUserService userService, IPostService postService) =>
    {
      // Check if user exists
      var user = await userService.GetAsync(id);
      if (user is null)
        return Results.NotFound();


      var userPosts = await postService.ListByUserAsync(id);

      // failure if user has posts
      // if (userPosts.Any())
      //   return Results.BadRequest("Cannot delete user with existing posts. Please delete all posts first.");

      // cascade delete posts
      foreach (var post in userPosts)
        await postService.DeleteAsync(post.Id);

      // Delete the user
      var deleted = await userService.DeleteAsync(id);
      return deleted ? Results.NoContent() : Results.NotFound();
    });

    // GET /users/{id:guid}/posts
    group.MapGet("/{id:guid}/posts", async (Guid id, IUserService userService, IPostService postService) =>
    {
      var user = await userService.GetAsync(id);
      if (user is null)
        return Results.NotFound();

      var posts = await postService.ListByUserAsync(id);
      var postDtos = posts.Select(p => new PostResponseDto(p.Id, p.UserId, p.Title, p.Content, p.PublishedAt));
      return Results.Ok(postDtos);
    });


    return group;
  }
}