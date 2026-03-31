using JournalApp.Services;
using JournalApp.Storage;
using JournalApp.Tests.Mocks;

namespace JournalApp.Tests;

public class JournalServiceTests
{
    [Theory]
    [InlineData("    hello      ", "hello")]
    [InlineData("What are you doing?", "What are you doing?")]
    [InlineData("   Here are   spaces   in the   middle too.", "Here are   spaces   in the   middle too.")]
    public async Task AddEntryAsync_TrimsAndSaves(string input, string expected)
    {
        IJournalStore fakeStore = new FakeJournalStore();
        var service = new JournalService(fakeStore);
        var entry = await service.AddEntryAsync(input);
        Assert.Equal(expected, entry.Content);
    }
    [Theory]
    [InlineData("")]
    [InlineData("      ")]
    public async Task AddEntryAsync_RejectsEmpty(string input)
    {
        IJournalStore fakeStore = new FakeJournalStore();
        var service = new JournalService(fakeStore);
        await Assert.ThrowsAsync<ArgumentException>(async () => await service.AddEntryAsync(input));
    }

    [Fact]
    public async Task AddEntryAsync_AcceptsCustomTimestamp()
    {
        IJournalStore fakeStore = new FakeJournalStore();
        var service = new JournalService(fakeStore);

        var timestamp = new DateTimeOffset(2026, 4, 1, 2, 18, 30, new TimeSpan(1, 0, 0));
        var entry = await service.AddEntryAsync("test", timestamp);

        Assert.Equal(timestamp, entry.Timestamp);
    }
    [Fact]
    public async Task SearchAsync_CaseInsensitiveContains()
    {
        IJournalStore fakeStore = new FakeJournalStore();
        var service = new JournalService(fakeStore);
        string[] testEntries = [
        "day",
        "Day" ,
        "dAy" ,
        "daY" ,
        "DAY" ,
        "something else"
        ];

        foreach (var test in testEntries)
        {
            await service.AddEntryAsync(test);
        }

        var searchResults = await service.SearchAsync("day");
        Assert.Equal(5, searchResults.Count);
        foreach (var result in searchResults)
        {

            Assert.Contains("day", result.Content.ToLower());
            Assert.DoesNotContain("something", result.Content.ToLower());
        }


    }
    [Fact]
    public async Task DeleteAsync_DeletesExisting()
    {
        IJournalStore fakeStore = new FakeJournalStore();
        var service = new JournalService(fakeStore);
        var entry = await service.AddEntryAsync("This is a test.");

        var wasDeleted = await service.DeleteAsync(entry.Id);

        var searchResults = await service.SearchAsync("test");

        Assert.True(wasDeleted);
        Assert.IsType<bool>(wasDeleted);
        Assert.Empty(searchResults);
    }
    [Fact]
    public async Task DeleteAsync_ReturnsFalseWhenNotFound()
    {
        IJournalStore fakeStore = new FakeJournalStore();
        var service = new JournalService(fakeStore);

        var wasDeleted = await service.DeleteAsync(Guid.NewGuid());

        Assert.False(wasDeleted);
        Assert.IsType<bool>(wasDeleted);
    }
}
