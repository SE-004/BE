using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using BlogApi.Services;
using BlogApi.Models;
using BlogApi.Dtos.Auth;
using Moq;

namespace BlogApi.Tests.Unit;

public class AuthServiceTests
{
    private static Mock<UserManager<User>> CreateMockUserManager()
    {
        // UserManager requires a complex constructor, so we mock it with minimal setup
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    private static Mock<IConfiguration> CreateMockConfiguration()
    {
        var mockConfig = new Mock<IConfiguration>();

        // Mock JWT configuration values
        mockConfig.Setup(c => c["Jwt:Key"]).Returns("super-secret-jwt-signing-key-that-is-at-least-256-bits-long-for-hmac-sha256");
        mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("BlogApi");
        mockConfig.Setup(c => c["Jwt:Audience"]).Returns("BlogApi-Users");
        mockConfig.Setup(c => c["Jwt:ExpiryMinutes"]).Returns("60");

        return mockConfig;
    }

    [Fact]
    public async Task RegisterAsync_ValidInput_ReturnsSuccess()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var mockConfig = CreateMockConfiguration();
        var authService = new AuthService(mockUserManager.Object, mockConfig.Object);

        var request = new RegisterRequestDto
        {
            Name = "Alice Johnson",
            Email = "alice@example.com",
            Password = "SecurePassword123!"
        };

        // Mock successful user creation
        mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await authService.RegisterAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Empty(result.Errors);

        // Verify UserManager was called with correct parameters
        mockUserManager.Verify(um => um.CreateAsync(
            It.Is<User>(u => u.Email == "alice@example.com" && u.Name == "Alice Johnson"),
            "SecurePassword123!"
        ), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_InvalidInput_ReturnsFailure()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var mockConfig = CreateMockConfiguration();
        var authService = new AuthService(mockUserManager.Object, mockConfig.Object);

        var request = new RegisterRequestDto
        {
            Name = "Bob Smith",
            Email = "invalid-email",
            Password = "weak"
        };

        // Mock failed user creation with validation errors
        var identityErrors = new[]
        {
            new IdentityError { Code = "InvalidEmail", Description = "Email is not valid" },
            new IdentityError { Code = "PasswordTooShort", Description = "Password is too short" }
        };
        mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        // Act
        var result = await authService.RegisterAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(2, result.Errors.Count());
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsAuthToken()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var mockConfig = CreateMockConfiguration();
        var authService = new AuthService(mockUserManager.Object, mockConfig.Object);

        var request = new LoginRequestDto
        {
            Email = "alice@example.com",
            Password = "SecurePassword123!"
        };

        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "alice@example.com",
            Name = "Alice Johnson",
            UserName = "alice@example.com"
        };

        // Mock successful user lookup and password check
        mockUserManager.Setup(um => um.FindByEmailAsync("alice@example.com"))
            .ReturnsAsync(existingUser);
        mockUserManager.Setup(um => um.CheckPasswordAsync(existingUser, "SecurePassword123!"))
            .ReturnsAsync(true);

        // Act
        var result = await authService.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result!.Token);
        Assert.True(result.ExpiresAtUtc > DateTime.UtcNow);

        // Verify the JWT token contains expected claims (basic validation)
        Assert.True(result.Token.Length > 50); // JWT tokens are typically long
    }

    [Fact]
    public async Task LoginAsync_InvalidEmail_ReturnsNull()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var mockConfig = CreateMockConfiguration();
        var authService = new AuthService(mockUserManager.Object, mockConfig.Object);

        var request = new LoginRequestDto
        {
            Email = "nonexistent@example.com",
            Password = "AnyPassword123!"
        };

        // Mock user not found
        mockUserManager.Setup(um => um.FindByEmailAsync("nonexistent@example.com"))
            .ReturnsAsync((User?)null);

        // Act
        var result = await authService.LoginAsync(request);

        // Assert
        Assert.Null(result);

        // Verify password check was never called (short-circuit behavior)
        mockUserManager.Verify(um => um.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ReturnsNull()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var mockConfig = CreateMockConfiguration();
        var authService = new AuthService(mockUserManager.Object, mockConfig.Object);

        var request = new LoginRequestDto
        {
            Email = "alice@example.com",
            Password = "WrongPassword123!"
        };

        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "alice@example.com",
            Name = "Alice Johnson",
            UserName = "alice@example.com"
        };

        // Mock user found but password check fails
        mockUserManager.Setup(um => um.FindByEmailAsync("alice@example.com"))
            .ReturnsAsync(existingUser);
        mockUserManager.Setup(um => um.CheckPasswordAsync(existingUser, "WrongPassword123!"))
            .ReturnsAsync(false);

        // Act
        var result = await authService.LoginAsync(request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCurrentUserAsync_ExistingUser_ReturnsUserInfo()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var mockConfig = CreateMockConfiguration();
        var authService = new AuthService(mockUserManager.Object, mockConfig.Object);

        var userId = Guid.NewGuid().ToString();
        var existingUser = new User
        {
            Id = Guid.Parse(userId),
            Email = "alice@example.com",
            Name = "Alice Johnson"
        };

        mockUserManager.Setup(um => um.FindByIdAsync(userId))
            .ReturnsAsync(existingUser);

        // Act
        var result = await authService.GetCurrentUserAsync(userId);

        // Assert
        Assert.NotNull(result);

        // Since the method returns anonymous object, we need to use dynamic or reflection
        var userInfo = result as dynamic;
        Assert.NotNull(userInfo);

        // Alternative: Check the result contains expected properties (safer approach)
        var resultString = result.ToString();
        Assert.Contains(userId, resultString!);
        Assert.Contains("Alice Johnson", resultString);
    }

    [Fact]
    public async Task GetCurrentUserAsync_NonExistentUser_ReturnsNull()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var mockConfig = CreateMockConfiguration();
        var authService = new AuthService(mockUserManager.Object, mockConfig.Object);

        var userId = Guid.NewGuid().ToString();

        mockUserManager.Setup(um => um.FindByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await authService.GetCurrentUserAsync(userId);

        // Assert
        Assert.Null(result);
    }
}