using BudgetApi.Dtos.Reports;


namespace BudgetApi.Application.Interfaces;

public interface IReportService
{
    Task<SummaryReportResponseDto> GetSummaryAsync(DateOnly start, DateOnly end, string type);
}