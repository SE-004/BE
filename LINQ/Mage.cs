namespace GameDemo.core
{
    public class Mage : Character
    {
        public int Mana { get; private set; } = 100;

        public Mage(string name)
            : base(name)
        {
            Console.WriteLine($"The mage {Name} has been born with {Mana} mana");
        }

        public void CastSpell(string spellName)
        {
            if (Mana >= 10)
            {
                Mana -= 10;
                Console.WriteLine($"{Name} casts {spellName}! (Mana left: {Mana})");
            }
            else
            {
                Console.WriteLine($"{Name} is out of mana");
            }
        }

        public override void Fight()
        {
            if (Mana >= 10)
            {
                CastSpell("Fireball");
            }
            else
            {
                Console.WriteLine($"{Name} is out of mana and just throws a punch");
            }
        }
    }
}
