using Microsoft.AspNetCore.Identity;

namespace BlogApi.Models;

public class User : IdentityUser<Guid>
{
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}