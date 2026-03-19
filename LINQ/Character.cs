namespace GameDemo.core
{
    public class Character
    {
        public static int Population { get; private set; } = 0;

        public string Name { get; set; }

        public int Level { get; private set; } = 1;

        public List<string> Inventory { get; private set; } = [];

        public Character(string name)
        {
            Name = name;
            Population++;
        }

        public void LevelUp()
        {
            Level++;
            Console.WriteLine($"{Name} is now level {Level}");
        }

        public virtual void Fight()
        {
            Console.WriteLine($"{Name} punches with bare hands");
        }
    }
}
