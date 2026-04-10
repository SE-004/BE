using Microsoft.EntityFrameworkCore;
using BlogApi.Models;

namespace BlogApi.Infrastructure.Data;

public class DbSeeder
{
  private readonly ApplicationDbContext _db;
  public DbSeeder(ApplicationDbContext db) => _db = db;

  public async Task SeedAsync(CancellationToken ct = default)
  {
    // Ensure database exists and is up to date
    await _db.Database.MigrateAsync(ct);

    if (!await _db.Users.AnyAsync(ct))
    {
      var alice = new User { Id = Guid.NewGuid(), Name = "Alice", Email = "alice@mail", CreatedAt = DateTimeOffset.UtcNow };
      var bob = new User { Id = Guid.NewGuid(), Name = "Bob", Email = "bob@mail", CreatedAt = DateTimeOffset.UtcNow };

      _db.Users.AddRange(alice, bob);
      await _db.SaveChangesAsync(ct);

      _db.Posts.AddRange(
                 new Post { Id = Guid.NewGuid(), UserId = alice.Id, Title = "Hello World", Content = "First post", PublishedAt = DateTimeOffset.UtcNow },
                 new Post { Id = Guid.NewGuid(), UserId = bob.Id, Title = "Notes", Content = "Second post", PublishedAt = null }
             );
      await _db.SaveChangesAsync(ct);
    }

  }
}