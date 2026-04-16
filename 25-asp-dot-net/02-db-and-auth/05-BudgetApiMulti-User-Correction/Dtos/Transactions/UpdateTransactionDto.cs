using System.ComponentModel.DataAnnotations;
using BudgetApi.Models;

namespace BudgetApi.Dtos.Transactions;

public record UpdateTransactionDto(

    TransactionType? Type,

    [property: StringLength(1_000, MinimumLength = 1)]
    string? Description,

    decimal? Amount,

    DateOnly? Date
);