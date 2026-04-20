using Microsoft.EntityFrameworkCore;
using BudgetApi.Infrastructure;
using BudgetApi.Application.Services;
using BudgetApi.Models;
using BudgetApi.Dtos.Transactions;

namespace BudgetApi.Tests.Unit;

public class ReportServiceTests
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
  public async Task GetSummaryAsync_CalculatesTransactionsInRange()
  {
    // Arrange - Set up test data and dependencies
    await using var db = CreateInMemoryDb();

    var transactionService = new TransactionServiceEf(db);
    var reportService = new ReportServiceEf(transactionService);

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

    var startDate = DateOnly.Parse("2025-11-01");
    var endDate = DateOnly.Parse("2025-12-01");

    CreateTransactionDto[] createDtosInRange = [
      new CreateTransactionDto(TransactionType.Income, "My first income in range", 100m, startDate),
      new CreateTransactionDto(TransactionType.Expense, "My first expense in range", 100m, DateOnly.Parse("2025-11-15")),
      new CreateTransactionDto(TransactionType.Income, "My second income in range", 100m, endDate),
    ];
    CreateTransactionDto[] createDtosOutOfRange = [
      new CreateTransactionDto(TransactionType.Income, "My first income out of range", 15m, DateOnly.Parse("2025-10-31")),
      new CreateTransactionDto(TransactionType.Expense, "My first expense out of range", 25m, DateOnly.Parse("2024-11-15")),
      new CreateTransactionDto(TransactionType.Income, "My second income out of range", 15m, DateOnly.Parse("2025-12-02")),
    ];



    // Act - Execute the method under test
    foreach (var dto in createDtosInRange)
      await transactionService.CreateAsync(user.Id, dto);

    foreach (var dto in createDtosOutOfRange)
      await transactionService.CreateAsync(user.Id, dto);


    var report = await reportService.GetSummaryAsync(startDate, endDate, "all");

    // // Assert - Verify the results
    Assert.Equal(startDate, report.StartDate);
    Assert.Equal(endDate, report.EndDate);
    Assert.Equal(200m, report.TotalIncome);
    Assert.Equal(100m, report.TotalExpense);
    Assert.Equal(100m, report.Net);
  }
}