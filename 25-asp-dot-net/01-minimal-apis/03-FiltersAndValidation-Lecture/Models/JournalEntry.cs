namespace FiltersLecture.Models;

public class JournalEntry
{
  public Guid Id { get; set; }
  public string Title { get; set; }
  public string Content { get; set; }
  public DateTime CreatedAt { get; set; }
}