using FiltersLecture.Dtos;
using FiltersLecture.Models;
using FiltersLecture.Filters;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/entries", (JournalEntryRequestDto entry) =>
{
  // No body validation by default on Minimal APIs
  var journalEntry = new JournalEntry
  {
    Id = Guid.NewGuid(),
    Title = entry.Title,
    Content = entry.Content,
    CreatedAt = DateTime.UtcNow
  };
  return Results.Created($"/entries/{journalEntry.Id}", journalEntry);
})
.WithValidation<JournalEntryRequestDto>();
// .AddEndpointFilter<ValidationFilter<JournalEntryRequestDto>>();

app.Run();