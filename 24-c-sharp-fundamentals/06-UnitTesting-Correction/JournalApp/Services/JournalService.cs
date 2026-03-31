using JournalApp.Models;
using JournalApp.Storage;

namespace JournalApp.Services;

public sealed class JournalService
{
  private readonly IJournalStore _store;

  public JournalService(IJournalStore store) => _store = store;

  public async Task<JournalEntry> AddEntryAsync(string content, DateTimeOffset? timestamp = null)
  {
    if (string.IsNullOrWhiteSpace(content))
      throw new ArgumentException("Content cannot be empty.", nameof(content));

    var entry = new JournalEntry
    {
      Content = content.Trim(),
      Timestamp = timestamp ?? DateTimeOffset.Now
    };
    await _store.SaveAsync(entry);
    return entry;
  }

  public Task<IReadOnlyList<JournalEntry>> SearchAsync(string keyword)
  {
    keyword ??= string.Empty;
    return _store.QueryAsync(e =>
        string.IsNullOrWhiteSpace(keyword) ||
        e.Content.Contains(keyword, StringComparison.OrdinalIgnoreCase));
  }

  public Task<bool> DeleteAsync(Guid id) => _store.DeleteAsync(id);
}