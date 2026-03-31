namespace JournalApp.Models;

public sealed class JournalEntry
{
  public Guid Id { get; init; } = Guid.NewGuid();
  public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.Now;
  public string Content { get; init; } = string.Empty;
}