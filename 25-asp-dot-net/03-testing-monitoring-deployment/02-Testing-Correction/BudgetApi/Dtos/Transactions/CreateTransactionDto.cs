using System.ComponentModel.DataAnnotations;
using BudgetApi.Models;

namespace BudgetApi.Dtos.Transactions;

public record CreateTransactionDto(
    [property: Required]
    TransactionType Type,

    [property: Required]
    [property: StringLength(1_000, MinimumLength = 1)]
    string Description,

    [property: Required]
    decimal Amount,

    DateOnly? Date
);