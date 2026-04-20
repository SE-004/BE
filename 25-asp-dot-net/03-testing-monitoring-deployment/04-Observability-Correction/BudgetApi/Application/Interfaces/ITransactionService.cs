using BudgetApi.Dtos.Transactions;
using BudgetApi.Models;

namespace BudgetApi.Application.Interfaces;

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> ListAsync();
    Task<Transaction?> GetAsync(Guid id);
    Task<Transaction> CreateAsync(Guid userId, CreateTransactionDto dto);
    Task<Transaction?> UpdateAsync(Guid id, UpdateTransactionDto dto);
    Task<bool> DeleteAsync(Guid id);
}