using Microsoft.EntityFrameworkCore;
using BudgetApi.Models;

namespace BudgetApi.Infrastructure.Data;

public class DbSeeder
{
  private readonly ApplicationDbContext _db;
  public DbSeeder(ApplicationDbContext db) => _db = db;

  public async Task SeedAsync(CancellationToken ct = default)
  {
    // Ensure database exists and is up to date
    await _db.Database.MigrateAsync(ct);

    if (!await _db.Transactions.AnyAsync(ct))
    {
      var income1 = new Transaction { Id = Guid.NewGuid(), Type = TransactionType.Income, Description = "October Salary", Amount = 2500m, Date = DateOnly.Parse("10/1/2025") };
      var income2 = new Transaction { Id = Guid.NewGuid(), Type = TransactionType.Income, Description = "November Salary", Amount = 2500m, Date = DateOnly.Parse("11/1/2025") };
      var income3 = new Transaction { Id = Guid.NewGuid(), Type = TransactionType.Income, Description = "October Salary", Amount = 2500m, Date = DateOnly.Parse("12/1/2025") };

      var expense1 = new Transaction { Id = Guid.NewGuid(), Type = TransactionType.Expense, Description = "October Rent", Amount = 1200m, Date = DateOnly.Parse("10/1/2025") };
      var expense2 = new Transaction { Id = Guid.NewGuid(), Type = TransactionType.Expense, Description = "November Rent", Amount = 1200m, Date = DateOnly.Parse("11/1/2025") };
      var expense3 = new Transaction { Id = Guid.NewGuid(), Type = TransactionType.Expense, Description = "December Rent", Amount = 1200m, Date = DateOnly.Parse("12/1/2025") };

      _db.Transactions.AddRange(income1, income2, income3, expense1, expense2, expense3);
      await _db.SaveChangesAsync(ct);

    }
  }
}