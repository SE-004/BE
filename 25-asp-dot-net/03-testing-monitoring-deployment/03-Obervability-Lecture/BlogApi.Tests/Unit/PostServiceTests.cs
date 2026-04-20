using Microsoft.EntityFrameworkCore;
using BlogApi.Infrastructure;
using BlogApi.Services;
using BlogApi.Models;

namespace BlogApi.Tests.Unit;

public class PostServiceTests
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
    public async Task CreateAsync_ValidUser_CreatesPostSuccessfully()
    {
        // Arrange - Set up test data and dependencies
        await using var db = CreateInMemoryDb();
        var userService = new UserService(db);
        var postService = new PostService(db);

        // Create a user first (posts require an existing user)
        var user = await userService.CreateAsync("Alice Johnson", "alice@example.com");

        // Act - Execute the method under test
        var post = await postService.CreateAsync(user.Id, "My First Post", "This is the content of my first post.");

        // Assert - Verify the results
        Assert.NotEqual(Guid.Empty, post.Id);
        Assert.Equal(user.Id, post.UserId);
        Assert.Equal("My First Post", post.Title);
        Assert.Equal("This is the content of my first post.", post.Content);
        Assert.True(post.PublishedAt <= DateTimeOffset.UtcNow);

        // Verify it was actually saved to the database
        var savedPost = await db.Posts.FindAsync(post.Id);
        Assert.NotNull(savedPost);
        Assert.Equal(post.Title, savedPost!.Title);
    }

    [Fact]
    public async Task CreateAsync_NonExistentUser_ThrowsArgumentException()
    {
        // Arrange
        await using var db = CreateInMemoryDb();
        var postService = new PostService(db);
        var fakeUserId = Guid.NewGuid(); // User that doesn't exist

        // Act & Assert - Verify exception is thrown
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => postService.CreateAsync(fakeUserId, "Post Title", "Post Content")
        );

        Assert.Equal("User not found (Parameter 'userId')", exception.Message);
    }

    [Fact]
    public async Task ListByUserAsync_ExistingUserWithPosts_ReturnsUserPosts()
    {
        // Arrange
        await using var db = CreateInMemoryDb();
        var userService = new UserService(db);
        var postService = new PostService(db);

        // Create two users
        var alice = await userService.CreateAsync("Alice", "alice@example.com");
        var bob = await userService.CreateAsync("Bob", "bob@example.com");

        // Create posts for both users
        await postService.CreateAsync(alice.Id, "Alice Post 1", "Content 1");
        await postService.CreateAsync(alice.Id, "Alice Post 2", "Content 2");
        await postService.CreateAsync(bob.Id, "Bob Post 1", "Content 3");

        // Act - Get posts for Alice only
        var alicePosts = await postService.ListByUserAsync(alice.Id);

        // Assert
        Assert.Equal(2, alicePosts.Count);
        Assert.All(alicePosts, post => Assert.Equal(alice.Id, post.UserId));
        Assert.Contains(alicePosts, p => p.Title == "Alice Post 1");
        Assert.Contains(alicePosts, p => p.Title == "Alice Post 2");
    }

    [Fact]
    public async Task UpdateAsync_ExistingPost_UpdatesSuccessfully()
    {
        // Arrange
        await using var db = CreateInMemoryDb();
        var userService = new UserService(db);
        var postService = new PostService(db);

        var user = await userService.CreateAsync("Author", "author@example.com");
        var post = await postService.CreateAsync(user.Id, "Original Title", "Original Content");

        // Act - Update only the title
        var updatedPost = await postService.UpdateAsync(post.Id, "Updated Title", null);

        // Assert
        Assert.NotNull(updatedPost);
        Assert.Equal("Updated Title", updatedPost!.Title);
        Assert.Equal("Original Content", updatedPost.Content); // Content unchanged
        
        // Verify changes were persisted
        var savedPost = await db.Posts.FindAsync(post.Id);
        Assert.Equal("Updated Title", savedPost!.Title);
    }

    [Fact]
    public async Task DeleteAsync_ExistingPost_DeletesSuccessfully()
    {
        // Arrange
        await using var db = CreateInMemoryDb();
        var userService = new UserService(db);
        var postService = new PostService(db);

        var user = await userService.CreateAsync("Author", "author@example.com");
        var post = await postService.CreateAsync(user.Id, "Post to Delete", "Content");

        // Act
        var result = await postService.DeleteAsync(post.Id);

        // Assert
        Assert.True(result);
        
        // Verify post no longer exists
        var deletedPost = await db.Posts.FindAsync(post.Id);
        Assert.Null(deletedPost);
    }

    [Fact]
    public async Task DeleteAsync_NonExistentPost_ReturnsFalse()
    {
        // Arrange
        await using var db = CreateInMemoryDb();
        var postService = new PostService(db);

        // Act
        var result = await postService.DeleteAsync(Guid.NewGuid());

        // Assert
        Assert.False(result);
    }
}