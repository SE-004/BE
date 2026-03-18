public class Goblin : IEnemy
{
    public void TakeDamage(int amount)
    {
        Console.WriteLine($"Goblin takes {amount} damage");
    }
}
