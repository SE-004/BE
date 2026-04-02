using BlogApi.Models;
using BlogApi.Services.Interfaces;

namespace BlogApi.Services;

public class InMemoryPostService : IPostService
{

  private readonly Dictionary<Guid, Post> _posts = new();
  private readonly IUserService _userService;

  public InMemoryPostService(IUserService userService)
  {
    _userService = userService;
  }
  public async Task<Post?> GetAsync(Guid id)
  {
    _posts.TryGetValue(id, out var post);
    return post;
  }
  public async Task<IReadOnlyList<Post>> ListAsync()
  {
    return _posts.Values.ToList();
  }

  public async Task<IReadOnlyList<Post>> ListByUserAsync(Guid userId)
  {
    return _posts.Values.Where(p => p.UserId == userId).ToList();
  }
  public async Task<Post> CreateAsync(Guid userId, string title, string content)
  {
    var user = await _userService.GetAsync(userId);

    if (user is null)
      throw new ArgumentException("User not found", nameof(userId));

    var post = new Post
    {
      Id = Guid.NewGuid(),
      UserId = userId,
      Title = title,
      Content = content,
      PublishedAt = DateTimeOffset.Now
    };

    _posts[post.Id] = post;

    return post;
  }
  public async Task<Post?> UpdateAsync(Guid id, string? title, string? content)
  {
    if (!_posts.TryGetValue(id, out var post))
      return null;

    if (title is not null)
      post.Title = title;

    if (content is not null)
      post.Content = content;

    return post;
  }
  public async Task<bool> DeleteAsync(Guid id)
  {
    return _posts.Remove(id);
  }
}