using BudgetApi.Models;
using BudgetApi.Application.Interfaces;
using BudgetApi.Dtos.Reports;


namespace BudgetApi.Application.Services;

public class ReportServiceEf(ITransactionService transactionService) : IReportService
{
  private readonly ITransactionService _transactionService = transactionService;

  public async Task<SummaryReportResponseDto> GetSummaryAsync(DateOnly start, DateOnly end, string type)
  {
    if (start > end) throw new ArgumentException("Start date must be before end date.");

    var transactions = await _transactionService.ListAsync();
    var requestedType = string.IsNullOrWhiteSpace(type) ? null : type;

    var transactionsInRange = transactions.Where(t =>
  {
    bool isInDateRange = t.Date >= start && t.Date <= end;

    if (requestedType is null) return isInDateRange;

    if (string.Equals(requestedType, "income", StringComparison.OrdinalIgnoreCase))
      return isInDateRange && t.Type == TransactionType.Income;

    if (string.Equals(requestedType, "expense", StringComparison.OrdinalIgnoreCase))
      return isInDateRange && t.Type == TransactionType.Expense;

    return isInDateRange;
  }).ToArray();

    var totalIncome = transactionsInRange.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
    var totalExpense = transactionsInRange.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
    var net = totalIncome - totalExpense;

    return new SummaryReportResponseDto(start, end, totalIncome, totalExpense, net);

  }


}