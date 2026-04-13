using Microsoft.EntityFrameworkCore;
using BudgetApi.Models;
using BudgetApi.Infrastructure;
using BudgetApi.Application.Interfaces;
using BudgetApi.Dtos.Transactions;

namespace BudgetApi.Application.Services;

public class TransactionServiceEf : ITransactionService
{
  private readonly ApplicationDbContext _db;

  public TransactionServiceEf(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<Transaction?> GetAsync(Guid id)
  {
    return await _db.Transactions.FindAsync(id);
  }

  public async Task<IEnumerable<Transaction>> ListAsync()
  {
    return await _db.Transactions.ToListAsync();
  }


  public async Task<Transaction> CreateAsync(CreateTransactionDto createTransactionDto)
  {

    var transaction = new Transaction
    {
      Type = createTransactionDto.Type,
      Description = createTransactionDto.Description,
      Amount = createTransactionDto.Amount,
      Date = createTransactionDto.Date ?? DateOnly.FromDateTime(DateTime.Now)
    };

    _db.Transactions.Add(transaction);
    await _db.SaveChangesAsync();
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