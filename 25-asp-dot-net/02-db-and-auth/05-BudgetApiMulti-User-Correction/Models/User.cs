using Microsoft.AspNetCore.Identity;

namespace BudgetApi.Models;

public class User : IdentityUser<Guid>
{
  public string Name { get; set; } = string.Empty;
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

  public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}