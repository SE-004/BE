using JournalApp.Services;
using JournalApp.Storage;

Console.OutputEncoding = System.Text.Encoding.UTF8;
var dataDir = Path.Combine(AppContext.BaseDirectory, "data");
Directory.CreateDirectory(dataDir);
var dbPath = Path.Combine(dataDir, "journal.json");

IJournalStore store = new JsonJournalStore(dbPath);
var service = new JournalService(store);

while (true)
{
  Console.WriteLine();
  Console.WriteLine("=== Journal ===");
  Console.WriteLine("1) Add entry");
  Console.WriteLine("2) Search entries (by keyword)");
  Console.WriteLine("3) Delete entry (by id)");
  Console.WriteLine("4) Exit");
  Console.Write("Choose: ");
  var choice = Console.ReadLine();

  try
  {
    switch (choice)
    {
      case "1":
        Console.Write("Write your entry: ");
        var content = Console.ReadLine() ?? string.Empty;
        var entry = await service.AddEntryAsync(content);
        Console.WriteLine($"Saved with id: {entry.Id}");
        break;
      case "2":
        Console.Write("Keyword: ");
        var term = Console.ReadLine() ?? string.Empty;
        var hits = await service.SearchAsync(term);
        if (!hits.Any()) { Console.WriteLine("No matches."); break; }
        foreach (var e in hits.OrderByDescending(e => e.Timestamp))
          Console.WriteLine($"{e.Timestamp:yyyy-MM-dd HH:mm} | {e.Id} | {e.Content}");
        break;
      case "3":
        Console.Write("Id (Guid): ");
        if (!Guid.TryParse(Console.ReadLine(), out var id))
          throw new ArgumentException("Invalid Guid.");
        var ok = await service.DeleteAsync(id);
        Console.WriteLine(ok ? "Deleted." : "Not found.");
        break;
      case "4":
        return;
      default:
        Console.WriteLine("Invalid choice.");
        break;
    }
  }
  catch (Exception ex)
  {
    Console.WriteLine($"Error: {ex.Message}");
  }
}