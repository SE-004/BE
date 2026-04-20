// BlogApi.Tests/Integration/UserEndpointsTests.cs
using System.Net;
using System.Net.Http.Json;
using BlogApi.Dtos.Users;

namespace BlogApi.Tests.Integration;

public class UserEndpointsTests : IClassFixture<TestHost>
{
    private readonly HttpClient _client;

    public UserEndpointsTests(TestHost factory)
    {
        _client = factory.CreateClient(); // Regular client for public endpoints
    }

    [Fact]
    public async Task GET_users_ReturnsEmptyList_WhenNoUsersExist()
    {
        // Act - Make HTTP request to public endpoint
        var response = await _client.GetAsync("/users");

        // Assert - Check response
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify response content
        var users = await response.Content.ReadFromJsonAsync<List<UserResponseDto>>();
        Assert.NotNull(users);
        Assert.Empty(users);
    }

    [Fact]
    public async Task GET_users_ReturnsUserList_WhenUsersExist()
    {
        // Arrange - Seed test data using HTTP requests (ensures same database)
        await SeedTestUser("Alice Johnson", "alice@unique1.com");
        await SeedTestUser("Bob Smith", "bob@unique2.com");

        // Act
        var response = await _client.GetAsync("/users");

        // Assert
        response.EnsureSuccessStatusCode();

        var users = await response.Content.ReadFromJsonAsync<List<UserResponseDto>>();
        Assert.NotNull(users);
        Assert.True(users!.Count >= 2); // May have data from other tests (shared DB)

        // Verify our test users are present
        Assert.Contains(users, u => u.Email == "alice@unique1.com");
        Assert.Contains(users, u => u.Email == "bob@unique2.com");
    }

    [Fact]
    public async Task POST_users_ValidInput_CreatesUserSuccessfully()
    {
        // Arrange
        var createUserDto = new CreateUserDto("Charlie Brown", "charlie@unique3.com");

        // Act - Make HTTP POST request
        var response = await _client.PostAsJsonAsync("/users", createUserDto);

        // Assert - Check response
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdUser = await response.Content.ReadFromJsonAsync<UserResponseDto>();
        Assert.NotNull(createdUser);
        Assert.Equal("Charlie Brown", createdUser!.Name);
        Assert.Equal("charlie@unique3.com", createdUser.Email);
        Assert.NotEqual(Guid.Empty, createdUser.Id);
        Assert.True(createdUser.CreatedAt <= DateTimeOffset.UtcNow);

        // Verify user was actually saved to database
        var allUsers = await _client.GetFromJsonAsync<List<UserResponseDto>>("/users");
        Assert.Contains(allUsers!, u => u.Email == "charlie@unique3.com");
    }

    [Fact]
    public async Task POST_users_InvalidEmail_ReturnsBadRequest()
    {
        // Arrange - Invalid email format
        var invalidUserDto = new CreateUserDto("Invalid User", "not-an-email");

        // Act
        var response = await _client.PostAsJsonAsync("/users", invalidUserDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        // Verify error response contains validation details
        var errorContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("email", errorContent.ToLower());
    }

    [Fact]
    public async Task POST_users_EmptyName_ReturnsBadRequest()
    {
        // Arrange - Empty name
        var invalidUserDto = new CreateUserDto("", "valid@unique4.com");

        // Act
        var response = await _client.PostAsJsonAsync("/users", invalidUserDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("name", errorContent.ToLower());
    }

    /// <summary>
    /// Helper method to seed test data by making HTTP requests (ensures same database)
    /// </summary>
    private async Task SeedTestUser(string name, string email)
    {
        var createUserDto = new CreateUserDto(name, email);
        var response = await _client.PostAsJsonAsync("/users", createUserDto);
        response.EnsureSuccessStatusCode(); // Ensure the user was created successfully
    }
}
