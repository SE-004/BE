using BudgetApi.Models;

namespace BudgetApi.Dtos.Transactions;

public record TransactionResponseDto(
  Guid Id,
  Guid UserId,
  DateTimeOffset Timestamp,
  TransactionType Type,
  string Description,
  decimal Amount,
  DateOnly Date
);