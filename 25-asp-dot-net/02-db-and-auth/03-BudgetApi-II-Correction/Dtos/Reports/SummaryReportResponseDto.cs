namespace BudgetApi.Dtos.Reports;

public record SummaryReportResponseDto(
  DateOnly StartDate,
  DateOnly EndDate,
  decimal TotalIncome,
  decimal TotalExpense,
  decimal Net
);