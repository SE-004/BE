using BlogApi.Models;
using BlogApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

public class PostService : IPostService
{
    private readonly ApplicationDbContext _db;

    public PostService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Post?> GetAsync(Guid id)
    {
        return await _db.Posts.FindAsync(id);
    }

    public async Task<IReadOnlyList<Post>> ListAsync()
    {
        return await _db.Posts.ToListAsync();
    }

    public async Task<IReadOnlyList<Post>> ListByUserAsync(Guid userId)
    {
        return await _db.Posts.Where(p => p.UserId == userId).ToListAsync();
    }

    public async Task<Post> CreateAsync(Guid userId, string title, string content)
    {
        // Validate that the user exists
        var user = await _db.Users.FindAsync(userId);
        if (user == null)
            throw new ArgumentException("User not found", nameof(userId));

        var post = new Post
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Content = content,
            PublishedAt = DateTimeOffset.UtcNow
        };

        _db.Posts.Add(post);
        await _db.SaveChangesAsync();
        return post;
    }

    public async Task<Post?> UpdateAsync(Guid id, string? title, string? content)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post == null)
            return null;

        if (title != null)
            post.Title = title;

        if (content != null)
            post.Content = content;

        await _db.SaveChangesAsync();
        return post;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post == null)
            return false;

        _db.Posts.Remove(post);
        await _db.SaveChangesAsync();
        return true;
    }

}