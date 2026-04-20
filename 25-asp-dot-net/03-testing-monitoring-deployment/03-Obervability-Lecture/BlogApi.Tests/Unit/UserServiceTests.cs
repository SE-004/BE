using Microsoft.EntityFrameworkCore;
using BlogApi.Infrastructure;
using BlogApi.Services;
using BlogApi.Models;

namespace BlogApi.Tests.Unit;

public class UserServiceTests
{
    /// <summary>
    /// Helper method to create a fresh InMemory database for each test.
    /// Using Guid.NewGuid() ensures each test gets its own isolated database.
    /// </summary>
    private static ApplicationDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB for each test
            .Options;
        
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ValidInput_CreatesUserSuccessfully()
    {
        // Arrange: Set up the test data and dependencies
        await using var db = CreateInMemoryDb();
        var userService = new UserService(db);
        
        // Act: Execute the method we're testing
        var user = await userService.CreateAsync("Alice Johnson", "alice@example.com");
        
        // Assert: Verify the expected outcomes
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("Alice Johnson", user.Name);
        Assert.Equal("alice@example.com", user.Email);
        Assert.True(user.CreatedAt <= DateTimeOffset.UtcNow);
        
        // Additional verification: Check the user was actually saved to the database
        var savedUser = await db.Users.FindAsync(user.Id);
        Assert.NotNull(savedUser);
        Assert.Equal(user.Name, savedUser!.Name);
    }

    [Fact]
    public async Task GetAsync_ExistingUser_ReturnsUser()
    {
        // Arrange: Create a user first
        await using var db = CreateInMemoryDb();
        var userService = new UserService(db);
        var createdUser = await userService.CreateAsync("Bob Smith", "bob@example.com");
        
        // Act: Try to retrieve the user
        var retrievedUser = await userService.GetAsync(createdUser.Id);
        
        // Assert: Verify we got the right user back
        Assert.NotNull(retrievedUser);
        Assert.Equal(createdUser.Id, retrievedUser!.Id);
        Assert.Equal("Bob Smith", retrievedUser.Name);
        Assert.Equal("bob@example.com", retrievedUser.Email);
    }

    [Fact]
    public async Task GetAsync_NonExistentUser_ReturnsNull()
    {
        // Arrange: Empty database
        await using var db = CreateInMemoryDb();
        var userService = new UserService(db);
        var randomId = Guid.NewGuid();
        
        // Act: Try to get a user that doesn't exist
        var user = await userService.GetAsync(randomId);
        
        // Assert: Should return null for non-existent user
        Assert.Null(user);
    }

    [Fact]
    public async Task DeleteAsync_ExistingUser_DeletesAndReturnsTrue()
    {
        // Arrange: Create a user to delete
        await using var db = CreateInMemoryDb();
        var userService = new UserService(db);
        var userToDelete = await userService.CreateAsync("Delete Me", "delete@example.com");
        
        // Act: Delete the user
        var result = await userService.DeleteAsync(userToDelete.Id);
        
        // Assert: Verify deletion was successful
        Assert.True(result);
        
        // Verify the user is actually gone from the database
        var deletedUser = await userService.GetAsync(userToDelete.Id);
        Assert.Null(deletedUser);
    }
}