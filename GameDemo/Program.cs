using GameDemo.core;

// var hero = new Character("Aragorn");
// var villain = new Character("Sauron");

// Console.WriteLine($"Total number of Characters: {Character.Population}");

// hero.Inventory.Add("sword");
// hero.Inventory.Add("potion");

// foreach (var item in hero.Inventory)
// {
//     Console.WriteLine($"{hero.Name} has: {item}");
// }

// hero.LevelUp();
// villain.LevelUp();

// ---- CREATE THE MAGE ----
// var gandalf = new Mage("Gandalf");
// gandalf.Inventory.Add("staff");
// gandalf.CastSpell("Fireball");
// gandalf.LevelUp();

// // ---- CREATE THE WARRIOR ----
// var conan = new Warrior("Conan");
// conan.LevelUp();
// conan.LevelUp();
// conan.LevelUp();
// conan.LevelUp();

// conan.Attack("Slash");

// Console.WriteLine($"Total number of Characters: {Character.Population}");

// var arenaFighters = new List<Character>
// {
//     new Mage("Gandalf"),
//     new Warrior("Conan"),
//     new Character("Villager"),
// };

// Console.WriteLine("--- BATTLE START ---");
// foreach (var fighter in arenaFighters)
// {
//     fighter.Fight();
// }

// var hero = new Character("Aragorn");

var merlin = new Mage("Merlin");

if (merlin is Character)
{
    Console.WriteLine("Merlin is a valid character");
}
