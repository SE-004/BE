using System.Text.Json;
using JournalApp.Models;

namespace JournalApp.Storage;

public sealed class JsonJournalStore : IJournalStore
{
  private readonly string _path;
  private readonly JsonSerializerOptions _json = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
  };

  public JsonJournalStore(string path)
  {
    _path = path ?? throw new ArgumentNullException(nameof(path));
    Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
    if (!File.Exists(_path)) File.WriteAllText(_path, "[]");
  }

  public async Task SaveAsync(JournalEntry entry, CancellationToken ct = default)
  {
    var list = (await ReadAllAsync(ct)).ToList();
    list.Add(entry);
    await WriteAllAsync(list, ct);
  }

  public async Task<IReadOnlyList<JournalEntry>> QueryAsync(Func<JournalEntry, bool> predicate, CancellationToken ct = default)
  {
    var list = await ReadAllAsync(ct);
    return list.Where(predicate).ToList();
  }

  public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
  {
    var list = (await ReadAllAsync(ct)).ToList();
    var removed = list.RemoveAll(e => e.Id == id) > 0;
    if (removed)
      await WriteAllAsync(list, ct);
    return removed;
  }

  private async Task<IReadOnlyList<JournalEntry>> ReadAllAsync(CancellationToken ct)
  {
    using var fs = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
    return (await JsonSerializer.DeserializeAsync<List<JournalEntry>>(fs, _json, ct))
           ?? new List<JournalEntry>();
  }

  private async Task WriteAllAsync(List<JournalEntry> list, CancellationToken ct)
  {
    using var fs = File.Open(_path, FileMode.Create, FileAccess.Write, FileShare.None);
    await JsonSerializer.SerializeAsync(fs, list, _json, ct);
  }
}