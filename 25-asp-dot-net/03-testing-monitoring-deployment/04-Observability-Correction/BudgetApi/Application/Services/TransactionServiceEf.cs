using Microsoft.EntityFrameworkCore;
using BudgetApi.Models;
using BudgetApi.Infrastructure;
using BudgetApi.Application.Interfaces;
using BudgetApi.Dtos.Transactions;

namespace BudgetApi.Application.Services;

public class TransactionServiceEf : ITransactionService
{
  private readonly ApplicationDbContext _db;
  private readonly ILogger<TransactionServiceEf> _logger;

  private readonly IMetricsService _metrics;

  public TransactionServiceEf(ApplicationDbContext db, ILogger<TransactionServiceEf> logger, IMetricsService metrics)
  {
    _db = db;
    _logger = logger;
    _metrics = metrics;
  }

  public async Task<Transaction?> GetAsync(Guid id)
  {
    _logger.LogInformation("Retrieving transaction with ID: {TransactionId}", id);
    return await _db.Transactions.FindAsync(id);
  }

  public async Task<IEnumerable<Transaction>> ListAsync()
  {
    return await _db.Transactions.ToListAsync();
  }


  public async Task<Transaction> CreateAsync(Guid userId, CreateTransactionDto createTransactionDto)
  {

    var transaction = new Transaction
    {
      UserId = userId,
      Type = createTransactionDto.Type,
      Description = createTransactionDto.Description,
      Amount = createTransactionDto.Amount,
      Date = createTransactionDto.Date ?? DateOnly.FromDateTime(DateTime.Now)
    };

    _db.Transactions.Add(transaction);
    await _db.SaveChangesAsync();
    _metrics.RecordTransactionCreated();
    return transaction;
  }

  public async Task<Transaction?> UpdateAsync(Guid id, UpdateTransactionDto updateTransactionDto)
  {
    var transaction = await _db.Transactions.FindAsync(id);
    if (transaction is null)
      return null;

    if (updateTransactionDto.Type is not null)
      transaction.Type = (TransactionType)updateTransactionDto.Type;

    if (updateTransactionDto.Description is not null)
      transaction.Description = updateTransactionDto.Description;

    if (updateTransactionDto.Amount is not null)
      transaction.Amount = (decimal)updateTransactionDto.Amount;

    if (updateTransactionDto.Date is not null)
      transaction.Date = (DateOnly)updateTransactionDto.Date;

    return transaction;
  }

  public async Task<bool> DeleteAsync(Guid id)
  {
    var transaction = await _db.Transactions.FindAsync(id);
    if (transaction is null)
      return false;

    _db.Transactions.Remove(transaction);
    await _db.SaveChangesAsync();
    return true;
  }
}