// JS
// const numbers = [1, 2, 3, 4, 5];
// numbers.filter(n => n > 3).map(n => n * 2)

// numbers.Where(n => n > 3).Select(n => n * 2);

// Goal               JavaScript                        C# LINQ
// ─────────────────────────────────────────────────────────────────────────
// Filter items       arr.filter(x => x > 5)            list.Where(x => x > 5)
// Transform items    arr.map(x => x.name)              list.Select(x => x.Name)
// Find one item      arr.find(x => x.id === 1)         list.FirstOrDefault(x => x.Id == 1)
// Check if exists    arr.some(x => x.isActive)          list.Any(x => x.IsActive)
// Check all          arr.every(x => x.isActive)         list.All(x => x.IsActive)
// Sort               arr.sort((a, b) => a - b)          list.OrderBy(x => x)
// Get unique         [...new Set(arr)]                  list.Distinct()
// Count items        arr.length                         list.Count

// JS
// const numbers = [1, 2, 3, 4, 5];
// const result = numbers.filter(n => n %2 === 0).map(n => n * n)
// [4, 16]

// C#
using GameDemo.core;

var numbers = new List<int> { 1, 2 };

// var result = numbers.Where(n => n %2 == 0).Select(n => n * n).ToList(); // Lazy evaluation

// Defining the query (nothing's happened yet)
var query = numbers.Select(n => n * 10);

// modify the original list
numbers.Add(3);

var result = query.ToList(); // 10, 20, 30

// var scores = new List<int> { 61, 10, 20, 80, 40, 70, 45, 100, 87 };

// var passingScores = scores.Where(score => score >= 60).OrderBy(n => n);

// foreach (int score in passingScores)
// {
//     Console.WriteLine($"Pass: {score}");
// }

// var characters = new List<Character>
// {
//     new Mage("Gandalf"),
//     new Warrior("Conan"),
//     new Mage("Saruman"),
// };

// var names = characters.Select(c => c.Name);

// foreach (var character in names)
// {
//     Console.WriteLine(character);
// }

// var summary = characters.Select(c => new { c.Name, Type = c.GetType().Name }).ToList();

// var summary = characters.Select(c => new CharacterSummary(c.Name, c.GetType().Name)).ToList();

// foreach (var entry in summary)
// {
//     Console.WriteLine($"{entry.Name} is a {entry.Type}");
// }

var scores = new List<int> { 61, 10, 20, 80, 40, 70, 45, 100, 87 };
var report = scores
    .OrderByDescending(s => s)
    .Select(s => new
    {
        Score = s,
        Passed = s >= 60,
        Grade = s switch
        {
            >= 90 => "A",
            >= 80 => "B",
            >= 70 => "C",
            >= 60 => "D",
            _ => "F",
        },
    })
    .ToList();

foreach (var entry in report)
{
    string status = entry.Passed ? "✅" : "❌";
    Console.WriteLine($"{status} Score: {entry.Score}, Grade: {entry.Grade}");
}

public record CharacterSummary(string Name, string Type);
