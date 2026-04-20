using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BudgetApi.Models;

namespace BudgetApi.Infrastructure.Data;

public class DbSeeder
{
  private readonly ApplicationDbContext _db;
  private readonly UserManager<User> _userManager;

  public DbSeeder(ApplicationDbContext db, UserManager<User> userManager)
  {
    _db = db;
    _userManager = userManager;
  }

  public async Task SeedAsync(CancellationToken ct = default)
  {
    // Ensure database exists and is up to date
    await _db.Database.MigrateAsync(ct);

    if (!await _userManager.Users.AnyAsync(ct))
    {
      var user1 = new User
      {
        UserName = "aang@air.com",
        Email = "aang@air.com",
        Name = "Aang Air"
      };
      var user2 = new User
      {
        UserName = "katara@water.com",
        Email = "katara@water.com",
        Name = "Katara Water"
      };

      await _userManager.CreateAsync(user1, "Aangpass123!");
      await _userManager.CreateAsync(user2, "Katarapass123!");

    }

    if (!await _db.Transactions.AnyAsync(ct))
    {
      // Fetch users to assign transactions
      var user1 = await _userManager.FindByEmailAsync("aang@air.com");
      var user2 = await _userManager.FindByEmailAsync("katara@water.com");

      // Guard against missing users (should exist after seeding above)
      if (user1 is null || user2 is null)
      {
        // If users aren't present, skip transaction seeding to avoid orphaned data
        return;
      }

      // Ensure each user has at least one income and one expense
      var income1 = new Transaction { Id = Guid.NewGuid(), UserId = user1.Id, Type = TransactionType.Income, Description = "October Salary", Amount = 2500m, Date = DateOnly.Parse("10/1/2025") };
      var income2 = new Transaction { Id = Guid.NewGuid(), UserId = user1.Id, Type = TransactionType.Income, Description = "November Salary", Amount = 2500m, Date = DateOnly.Parse("11/1/2025") };
      var income3 = new Transaction { Id = Guid.NewGuid(), UserId = user2.Id, Type = TransactionType.Income, Description = "December Salary", Amount = 2500m, Date = DateOnly.Parse("12/1/2025") };

      var expense1 = new Transaction { Id = Guid.NewGuid(), UserId = user1.Id, Type = TransactionType.Expense, Description = "October Rent", Amount = 1200m, Date = DateOnly.Parse("10/1/2025") };
      var expense2 = new Transaction { Id = Guid.NewGuid(), UserId = user2.Id, Type = TransactionType.Expense, Description = "November Rent", Amount = 1200m, Date = DateOnly.Parse("11/1/2025") };
      var expense3 = new Transaction { Id = Guid.NewGuid(), UserId = user2.Id, Type = TransactionType.Expense, Description = "December Rent", Amount = 1200m, Date = DateOnly.Parse("12/1/2025") };

      _db.Transactions.AddRange(income1, expense1, income2, income3, expense2, expense3);
      await _db.SaveChangesAsync(ct);

    }
  }
}