using Microsoft.EntityFrameworkCore;
using BlogApi.Infrastructure;
using BlogApi.Services;
using BlogApi.Models;

namespace BlogApi.Tests.Unit;

public class UserServiceTests
{
  // Helper method to create a fresh in-memory database for each test
  private static ApplicationDbContext CreateInMemoryDb()
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB per test
        .Options;
    return new ApplicationDbContext(options);
  }

  [Fact]
  public async Task CreateAsync_ValidInput_CreatesUserSuccessfully()
  {
    // Arrange - Set up test data and dependencies
    await using var db = CreateInMemoryDb();
    var service = new UserService(db);

    // Act - Execute the method under test
    var userName = "Alice Johnson";
    var userEmail = "alice@example.com";
    var user = await service.CreateAsync(userName, userEmail);

    // Assert - Verify the results
    Assert.NotEqual(Guid.Empty, user.Id);
    Assert.Equal(userName, user.Name);
    Assert.Equal(userEmail, user.Email);
    Assert.True(user.CreatedAt <= DateTimeOffset.UtcNow);

    // Verify it was actually saved to the database
    var savedUser = await db.Users.FindAsync(user.Id);
    Assert.NotNull(savedUser);
    Assert.Equal(user.Name, savedUser!.Name);


  }

  [Fact]
  public async Task GetAsync_ExistingUser_ReturnsUser()
  {
    // Arrange
    await using var db = CreateInMemoryDb();
    var service = new UserService(db);
    var created = await service.CreateAsync("Bob Smith", "bob@example.com");

    // Act
    var retrieved = await service.GetAsync(created.Id);

    // Assert
    Assert.NotNull(retrieved);
    Assert.Equal(created.Id, retrieved!.Id);
    Assert.Equal("Bob Smith", retrieved.Name);
  }

  [Fact]
  public async Task GetAsync_NonExistentUser_ReturnsNull()
  {
    // Arrange
    await using var db = CreateInMemoryDb();
    var service = new UserService(db);

    // Act
    var user = await service.GetAsync(Guid.NewGuid());

    // Assert
    Assert.Null(user);
  }
}