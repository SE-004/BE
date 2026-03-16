namespace GameDemo.core
{
    public class Warrior : Character
    {
        public int Stamina { get; private set; } = 100;

        public Warrior(string name)
            : base(name)
        {
            Console.WriteLine($"The warrior {Name} has been born with {Stamina} stamina");
        }

        public void Attack(string move)
        {
            if (Stamina >= 15)
            {
                Stamina -= 15;
                Console.WriteLine($"{Name} attacks with {move}! (Stamina left: {Stamina})");
            }
            else
            {
                Console.WriteLine($"{Name} is too exhausted to attack");
            }
        }

        public override void Fight()
        {
            if (Stamina >= 15)
            {
                Attack("Slash");
            }
            else
            {
                Console.WriteLine($"{Name} is too exhausted and just throws a punch");
            }
        }
    }
}
