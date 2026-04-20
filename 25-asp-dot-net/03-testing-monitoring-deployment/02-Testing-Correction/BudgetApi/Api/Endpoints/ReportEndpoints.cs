
using BudgetApi.Application.Interfaces;
using BudgetApi.Dtos.Reports;


namespace BudgetApi.Api.Endpoints;

public static class ReportEndpoints
{
  public static void MapReportEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/reports").WithTags("Reports");


    group.MapGet("/summary", async (IReportService reportService, DateOnly start, DateOnly end, string type) =>
    {
      var reportDto = await reportService.GetSummaryAsync(start, end, type);
      return TypedResults.Ok(reportDto);
    })
    .Produces<IEnumerable<SummaryReportResponseDto>>();


  }
}