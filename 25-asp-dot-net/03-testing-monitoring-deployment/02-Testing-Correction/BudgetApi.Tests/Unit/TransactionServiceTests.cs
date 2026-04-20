using Microsoft.EntityFrameworkCore;
using BudgetApi.Infrastructure;
using BudgetApi.Application.Services;
using BudgetApi.Models;
using BudgetApi.Dtos.Transactions;

namespace BudgetApi.Tests.Unit;

public class TransactionServiceTests
{
  // Helper method to create a fresh in-memory database for each test
  private static ApplicationDbContext CreateInMemoryDb()
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB per test
        .Options;
    return new ApplicationDbContext(options);
  }

  [Fact]
  public async Task CreateAsync_ValidUser_CreatesTransactionSuccessfully()
  {
    // Arrange - Set up test data and dependencies
    await using var db = CreateInMemoryDb();

    var transactionService = new TransactionServiceEf(db);

    // Seed a user directly into the in-memory database (no AuthService dependency)
    var user = new User
    {
      Id = Guid.NewGuid(),
      Name = "Alice Johnson",
      Email = "alice@example.com",
      UserName = "alice@example.com"
    };
    db.Users.Add(user);
    await db.SaveChangesAsync();

    var description = "My first income.";
    var amount = 100.00m;
    var date = DateOnly.Parse("2025-12-01");

    var createDto = new CreateTransactionDto(TransactionType.Income, description, amount, date);

    // Act - Execute the method under test
    var transaction = await transactionService.CreateAsync(user.Id, createDto);

    // Assert - Verify the results
    Assert.NotEqual(Guid.Empty, transaction.Id);
    Assert.Equal(user.Id, transaction.UserId);
    Assert.Equal(TransactionType.Income, transaction.Type);
    Assert.Equal(description, transaction.Description);
    Assert.Equal(amount, transaction.Amount);
    Assert.Equal(date, transaction.Date);

    // Verify it was actually saved to the database
    var savedTransaction = await db.Transactions.FindAsync(transaction.Id);
    Assert.NotNull(savedTransaction);
    Assert.Equal(transaction.Description, savedTransaction!.Description);
  }
}