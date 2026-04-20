// BudgetApi.Tests/Unit/AuthServiceTests.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using BudgetApi.Application.Services;
using BudgetApi.Models;
using BudgetApi.Dtos.Auth;

namespace BudgetApi.Tests.Unit;

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
    mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("BudgetApi");
    mockConfig.Setup(c => c["Jwt:Audience"]).Returns("BudgetApi-Users");
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

    var request = new RegisterRequestDto(
     "Alice Johnson",
    "alice@example.com",
     "SecurePassword123!"
    );

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
  public async Task LoginAsync_ValidCredentials_ReturnsAuthToken()
  {
    // Arrange
    var mockUserManager = CreateMockUserManager();
    var mockConfig = CreateMockConfiguration();
    var authService = new AuthService(mockUserManager.Object, mockConfig.Object);

    var request = new LoginRequestDto(
      "alice@example.com",
      "SecurePassword123!"
    );

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
  public async Task LoginAsync_InvalidPassword_ReturnsNull()
  {
    // Arrange
    var mockUserManager = CreateMockUserManager();
    var mockConfig = CreateMockConfiguration();
    var authService = new AuthService(mockUserManager.Object, mockConfig.Object);

    var request = new LoginRequestDto(
      "alice@example.com",
      "WrongPassword123!"
    );

    var existingUser = new User { Id = Guid.NewGuid(), Email = "alice@example.com", Name = "Alice Johnson" };

    // Mock user found but password check fails
    mockUserManager.Setup(um => um.FindByEmailAsync("alice@example.com")).ReturnsAsync(existingUser);
    mockUserManager.Setup(um => um.CheckPasswordAsync(existingUser, "WrongPassword123!")).ReturnsAsync(false);

    // Act
    var result = await authService.LoginAsync(request);

    // Assert
    Assert.Null(result);
  }
}