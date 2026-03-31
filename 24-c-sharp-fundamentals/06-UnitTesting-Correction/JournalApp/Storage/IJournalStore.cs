using JournalApp.Models;

namespace JournalApp.Storage;

public interface IJournalStore
{
  Task SaveAsync(JournalEntry entry, CancellationToken ct = default);
  Task<IReadOnlyList<JournalEntry>> QueryAsync(Func<JournalEntry, bool> predicate, CancellationToken ct = default);
  Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}