// static void DamageEnemy(IEnemy enemy, int amount)
// {
//     enemy.TakeDamage(amount);
// }

// var goblin = new Goblin();
// var dragon = new Dragon();

// DamageEnemy(dragon, 4);
// DamageEnemy(goblin, 20);

// var names = new List<string> { "Bob", "Jake", "Timmy" };
// IEnumerable<string> enumerable = names;

// foreach (var name in enumerable)
// {
//     Console.WriteLine(name);
// }

// var names = new List<string> { "Bob", "Jake", "Timmy" };
// ICollection<string> collection = names;

// foreach (var name in collection)
// {
//     Console.WriteLine(name);
// }

// collection.Add("");
// collection.Remove("");
// int count = collection.Count;
// bool exists = collection.Contains("Bob");

// static void PrintNames(List<string> names)
// {
//     foreach (var name in names)
//     {
//         Console.WriteLine(name);
//     }
// }

// static void PrintNames(IEnumerable<string> names)
// {
//     foreach (var name in names)
//     {
//         Console.WriteLine(name);
//     }
// }

// PrintNames(new List<string> { "Bob", "Timmy" }); // List
// PrintNames(new string[] { "Bob", "Timmy" }); // Array

static void AddNameIfDoesntExist(ICollection<string> names, string newName)
{
    if (!names.Contains(newName))
    {
        names.Add(newName);
    }
}

var list = new List<string> { "Bob", "Timmy" };

AddNameIfDoesntExist(list, "Jake");
AddNameIfDoesntExist(list, "Timmy");

foreach (var name in list)
{
    Console.WriteLine(name);
}
