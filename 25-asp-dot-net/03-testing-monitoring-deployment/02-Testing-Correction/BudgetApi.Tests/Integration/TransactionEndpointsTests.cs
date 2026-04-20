// BudgetApi.Tests/Integration/UserEndpointsTests.cs
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using BudgetApi.Infrastructure;
using BudgetApi.Dtos.Transactions;
using BudgetApi.Models;

namespace BudgetApi.Tests.Integration;

public class TransactionEndpointsTests : IClassFixture<TestHost>
{
  private readonly HttpClient _client;
  private readonly TestHost _factory;

  public TransactionEndpointsTests(TestHost factory)
  {
    _factory = factory;
    _client = factory.CreateClient(); // Regular client for public endpoints
  }

  [Fact]
  public async Task POST_transactions_Returns401Error_WhenHeadersMissing()
  {

    // await SeedTestUser("Alice Johnson", "alice@unique1.com", "Alicepass123!");

    var createTransactionDto = new CreateTransactionDto(TransactionType.Income, "Test income", 10m, DateOnly.Parse("2025-12-01"));

    // Act - Make HTTP request to private endpoint
    var response = await _client.PostAsJsonAsync("/api/transactions", createTransactionDto);

    // Assert - Check response
    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);


  }
  [Fact]
  public async Task POST_transactions_ReturnsTransaction_WithValidHeaders()
  {


    var description = "Test Income";
    var amount = 10m;
    var date = DateOnly.Parse("2025-12-01");

    var createTransactionDto = new CreateTransactionDto(TransactionType.Income, description, amount, date);

    // var userId = Guid.NewGuid(); // or the seeded user's id
    var userId = await SeedTestUser("Alice Johnson", "alice@unique1.com");

    // Act - Make HTTP request to private endpoint
    var request = new HttpRequestMessage(HttpMethod.Post, "/api/transactions");
    request.Headers.Add("X-Test-UserId", userId.ToString());
    request.Headers.Add("X-Test-Email", "alice@unique1.com");
    request.Headers.Add("X-Test-Name", "Alice Johnson");

    request.Content = JsonContent.Create(createTransactionDto); // System.Net.Http.Json

    var response = await _client.SendAsync(request);
    response.EnsureSuccessStatusCode();

    // Assert - Check response
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    var createdTransaction = await response.Content.ReadFromJsonAsync<TransactionResponseDto>();

    Assert.Equal(userId, createdTransaction!.UserId);
    Assert.Equal(TransactionType.Income, createdTransaction.Type);
    Assert.Equal(description, createdTransaction.Description);
    Assert.Equal(amount, createdTransaction.Amount);
    Assert.Equal(date, createdTransaction.Date);

  }


  /// <summary>
  /// Helper method to seed test data by creating scope to ensure same DB (like with the DB Seeder)
  /// </summary>
  private async Task<Guid> SeedTestUser(string name, string email)
  {
    using var scope = _factory.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var user = new User
    {
      Id = Guid.NewGuid(),
      Name = name,
      Email = email,
      UserName = email
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return user.Id;
  }
}