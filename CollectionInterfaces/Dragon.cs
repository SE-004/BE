public class Dragon : IEnemy
{
    public void TakeDamage(int amount)
    {
        Console.WriteLine($"Dragon takes {amount} damage");
    }
}
