using BudgetApi.Models;
using BudgetApi.Application.Interfaces;
using BudgetApi.Dtos.Transactions;

namespace BudgetApi.Application.Services;

public class TransactionServiceMock : ITransactionService
{
    private readonly Dictionary<Guid, Transaction> _transactions = new();

    public Task<Transaction?> GetAsync(Guid id)
    {
        _transactions.TryGetValue(id, out var transaction);
        return Task.FromResult(transaction);
    }

    public Task<IEnumerable<Transaction>> ListAsync()
    {
        return Task.FromResult<IEnumerable<Transaction>>(_transactions.Values.ToList());
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

        _transactions[transaction.Id] = transaction;
        return transaction;
    }

    public Task<Transaction?> UpdateAsync(Guid id, UpdateTransactionDto updateTransactionDto)
    {
        if (!_transactions.TryGetValue(id, out var transaction))
            return Task.FromResult<Transaction?>(null);

        if (updateTransactionDto.Type is not null)
            transaction.Type = (TransactionType)updateTransactionDto.Type;

        if (updateTransactionDto.Description is not null)
            transaction.Description = updateTransactionDto.Description;

        if (updateTransactionDto.Amount is not null)
            transaction.Amount = (decimal)updateTransactionDto.Amount;

        if (updateTransactionDto.Date is not null)
            transaction.Date = (DateOnly)updateTransactionDto.Date;

        return Task.FromResult<Transaction?>(transaction);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return Task.FromResult(_transactions.Remove(id));
    }
}