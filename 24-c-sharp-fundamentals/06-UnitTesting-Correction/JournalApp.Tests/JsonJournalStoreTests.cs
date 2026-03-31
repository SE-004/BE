using JournalApp.Storage;
using JournalApp.Models;

namespace JournalApp.Tests;

public class JsonJournalStoreTests
{

  [Fact]
  public async Task SaveAsync_CanBeQueriedWith_QueryAsync()
  {
    var dataDir = Path.Combine(AppContext.BaseDirectory, "test-data");
    if (Directory.Exists(dataDir))
    {
      Directory.Delete(dataDir, recursive: true);
    }

    Directory.CreateDirectory(dataDir);
    var dbPath = Path.Combine(dataDir, "test-journal.json");

    var jsonStore = new JsonJournalStore(dbPath);
    var entryContent = "This is a test.";
    var entry = new JournalEntry { Content = entryContent };

    await jsonStore.SaveAsync(entry);

    var queryResult = await jsonStore.QueryAsync(e =>
        e.Content.Contains("this", StringComparison.OrdinalIgnoreCase));

    Assert.Single(queryResult);
    Assert.Equal(entryContent, queryResult[0].Content);

    if (Directory.Exists(dataDir))
    {
      Directory.Delete(dataDir, recursive: true);
    }
  }
  [Fact]
  public async Task DeleteAsync_DeletesEntry()
  {
    var dataDir = Path.Combine(AppContext.BaseDirectory, "test-data");
    if (Directory.Exists(dataDir))
    {
      Directory.Delete(dataDir, recursive: true);
    }

    Directory.CreateDirectory(dataDir);
    var dbPath = Path.Combine(dataDir, "test-journal.json");

    var jsonStore = new JsonJournalStore(dbPath);
    var entryContent = "This is a test.";
    var entry = new JournalEntry { Content = entryContent };

    await jsonStore.SaveAsync(entry);

    var wasDeleted = await jsonStore.DeleteAsync(entry.Id);

    var entriesAfterDeletion = await jsonStore.QueryAsync(e => true);

    Assert.True(wasDeleted);
    Assert.Empty(entriesAfterDeletion);

    if (Directory.Exists(dataDir))
    {
      Directory.Delete(dataDir, recursive: true);
    }
  }
  [Fact]
  public async Task SaveAsync_SavesAllInOrder()
  {
    var dataDir = Path.Combine(AppContext.BaseDirectory, "test-data");
    if (Directory.Exists(dataDir))
    {
      Directory.Delete(dataDir, recursive: true);
    }

    Directory.CreateDirectory(dataDir);
    var dbPath = Path.Combine(dataDir, "test-journal.json");

    var jsonStore = new JsonJournalStore(dbPath);

    string[] entryContents = [
    "This is the first entry.",
    "This is the second entry.",
    "This is the third entry."
    ];

    JournalEntry[] entries = [
          new() { Content = entryContents[0] },
          new() { Content = entryContents[1] },
          new() { Content = entryContents[2] }
        ];

    foreach (var entry in entries)
    {
      await jsonStore.SaveAsync(entry);
    }

    var savedEntries = await jsonStore.QueryAsync(e => true);

    Assert.Equal(3, savedEntries.Count);
    Assert.Collection(savedEntries,
      first => Assert.Equal(entryContents[0], first.Content),
      second => Assert.Equal(entryContents[1], second.Content),
      third => Assert.Equal(entryContents[2], third.Content)
      );

    if (Directory.Exists(dataDir))
    {
      Directory.Delete(dataDir, recursive: true);
    }
  }
}