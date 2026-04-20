namespace BudgetApi.Models;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
    public TransactionType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }

    public User? User { get; set; }

}