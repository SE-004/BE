using System.Net;
using System.Net.Http.Json;
using BlogApi.Dtos.Posts;
using BlogApi.Dtos.Users;

namespace BlogApi.Tests.Integration;

public class PostEndpointsTests : IClassFixture<TestHost>
{
  private readonly HttpClient _client;

  public PostEndpointsTests(TestHost factory)
  {
    _client = factory.CreateClient(); // Regular client for public endpoints
  }

  [Fact]
  public async Task POST_posts_ValidInputAndHeaders_CreatesPostSuccessfully()
  {
    var createUserDto = new CreateUserDto("Alice Johnson", "alice@unique2.com");
    var userResponse = await _client.PostAsJsonAsync("/users", createUserDto);
    userResponse.EnsureSuccessStatusCode();
    var createdUser = await userResponse.Content.ReadFromJsonAsync<UserResponseDto>();
    var userId = createdUser!.Id;

    var title = "Test Post";
    var content = "Content of test post";
    var createPostDto = new CreatePostDto(title, content);

    var request = new HttpRequestMessage(HttpMethod.Post, "/posts");
    request.Headers.Add("X-Test-UserId", userId.ToString());
    request.Headers.Add("X-Test-Email", "alice@unique2.com");
    request.Headers.Add("X-Test-Name", "Alice Johnson");


    request.Content = JsonContent.Create(createPostDto);

    var response = await _client.SendAsync(request);

    var createdPost = await response.Content.ReadFromJsonAsync<PostResponseDto>();
    Assert.NotNull(createdPost);
    Assert.Equal(title, createdPost!.Title);
    Assert.Equal(content, createdPost.Content);
    Assert.Equal(userId, createdPost.UserId);
    Assert.NotEqual(Guid.Empty, createdPost.Id);
    Assert.True(createdPost.PublishedAt <= DateTimeOffset.UtcNow);

    // Verify user was actually saved to database
    var allPosts = await _client.GetFromJsonAsync<List<PostResponseDto>>("/posts");
    Assert.Contains(allPosts!, p => p.Title == title);
  }

}