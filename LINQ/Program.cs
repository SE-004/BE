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
// using GameDemo.core;

// var numbers = new List<int> { 1, 2 };

// var result = numbers.Where(n => n %2 == 0).Select(n => n * n).ToList(); // Lazy evaluation

// Defining the query (nothing's happened yet)
// var query = numbers.Select(n => n * 10);

// modify the original list
// numbers.Add(3);

// var result = query.ToList(); // 10, 20, 30

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

// var scores = new List<int> { 61, 10, 20, 80, 40, 70, 45, 100, 87 };
// var report = scores
//     .OrderByDescending(s => s)
//     .Select(s => new
//     {
//         Score = s,
//         Passed = s >= 60,
//         Grade = s switch
//         {
//             >= 90 => "A",
//             >= 80 => "B",
//             >= 70 => "C",
//             >= 60 => "D",
//             _ => "F",
//         },
//     })
//     .ToList();

// foreach (var entry in report)
// {
//     string status = entry.Passed ? "✅" : "❌";
//     Console.WriteLine($"{status} Score: {entry.Score}, Grade: {entry.Grade}");
// }

// public record CharacterSummary(string Name, string Type);

// --------------------------------- JOINS & GROUPING ---------------------------------

// Grouping with .GroupBy()

//SQL
// SELECT Department, COUNT(*) FROM Employees GROUP BY Department

// var scores = new List<(string Name, int Score)>
// {
//     ("Bob", 61),
//     ("Jake", 10),
//     ("Jeff", 20),
//     ("Carl", 80),
//     ("Ben", 40),
//     ("Mark", 70),
//     ("Marie", 45),
//     ("Julia", 100),
//     ("Madelein", 87),
// };

// var grouped = scores.GroupBy(s => s.Score >= 60 ? "Pass" : "Fail");

// foreach (var group in grouped)
// {
//     Console.WriteLine($"--- {group.Key} ---");
//     foreach (var (Name, Score) in group)
//     {
//         Console.WriteLine($"{Name}: {Score}");
//     }
// }

// var people = new List<(string Name, string Department)>
// {
//     ("Carl", "Engineering"),
//     ("Ben", "Marketing"),
//     ("Mark", "Engineering"),
//     ("Marie", "Marketing"),
//     ("Julia", "Engineering"),
//     ("Madelein", "HR"),
// };

// var byDepartment = people.GroupBy(p => p.Department);

// foreach (var group in byDepartment)
// {
//     Console.WriteLine($"--- {group.Key}: {group.Count()} people ---");

//     foreach (var person in group)
//     {
//         Console.WriteLine($"{person.Name}");
//     }
// }

// var summary = people
//     .GroupBy(p => p.Department)
//     .Select(g => new
//     {
//         Department = g.Key,
//         Count = g.Count(),
//         Members = string.Join(", ", g.Select(p => p.Name)),
//     })
//     .OrderByDescending(s => s.Count)
//     .ToList();

// foreach (var dept in summary)
// {
//     Console.WriteLine($"{dept.Department} ({dept.Count}): {dept.Members}");
// }

// Ideally, put multiple values into scores to get a better result for the Average and Max
// var scores = new List<(string Name, string Subject, int Score)>
// {
//     ("Bob", "Math", 61),
//     ("Jake", "Biology", 10),
//     ("Jeff", "Math", 20),
//     ("Carl", "Science", 80),
//     ("Ben", "Biology", 40),
//     ("Mark", "Science", 70),
//     ("Marie", "Math", 45),
//     ("Julia", "Science", 100),
//     ("Madelein", "Biology", 87),
// };

// var averages = scores
//     .GroupBy(s => s.Name)
//     .Select(g => new
//     {
//         Student = g.Key,
//         Average = g.Average(s => s.Score),
//         Highest = g.Max(s => s.Score),
//     })
//     .OrderByDescending(s => s.Average)
//     .ToList();

// foreach (var student in averages)
// {
//     Console.WriteLine($"{student.Student}: avg {student.Average}, best {student.Highest}");
// }

var students = new List<(int Id, string Name)>
{
    (1, "Bob"),
    (2, "Fiona"),
    (3, "Mark"),
    (4, "Anna"),
};

var enrollments = new List<(int StudentId, string Course)>
{
    (1, "Math"),
    (1, "Art"),
    (3, "Art"),
    (1, "Science"),
    (4, "Math"),
    (2, "Math"),
    (4, "Science"),
};

// IN SQL:
// SELECT s.Name, e.Course
// FROM Students s
// INNER JOIN Enrollments e
// ON s.Id = e.StudentId

// Equivalent in C#:
// var joined = students
//     .Join( // first collection
//         enrollments, // second collection
//         s => s.Id, // key from first collection
//         e => e.StudentId, // key from second collection
//         (s, e) => new { s.Name, e.Course } // what to do with each match
//     )
//     .GroupBy(x => x.Name)
//     .Select(g => new { Student = g.Key, Courses = string.Join(", ", g.Select(x => x.Course)) });

// foreach (var entry in joined)
// {
//     Console.WriteLine($"{entry.Student}: {entry.Courses}");
// }

var products = new List<(int Id, string Name, string Category)>
{
    (1, "Laptop", "Electronics"),
    (2, "Phone", "Electronics"),
    (3, "Chair", "Furniture"),
    (4, "Bed", "Furniture"),
    (5, "Pasta", "Food"),
};

var orders = new List<(int ProductId, int Quantity)>
{
    (1, 1),
    (2, 1),
    (2, 2),
    (5, 18),
    (4, 1),
    (3, 10),
    (5, 2),
};

var report = orders
    .Join(
        products,
        o => o.ProductId,
        p => p.Id,
        (o, p) =>
            new
            {
                p.Name,
                p.Category,
                o.Quantity,
            }
    )
    .GroupBy(p => p.Category)
    .Select(g => new
    {
        Category = g.Key,
        TotalItems = g.Sum(x => x.Quantity),
        Products = string.Join(", ", g.Select(x => x.Name).Distinct()),
    })
    .OrderByDescending(x => x.TotalItems)
    .ToList();

foreach (var entry in report)
{
    Console.WriteLine($"{entry.Category}: {entry.TotalItems} items ({entry.Products})");
}
