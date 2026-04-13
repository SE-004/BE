
using Scalar.AspNetCore;
using BudgetApi.Api.Endpoints;
using BudgetApi.Application.Services;
using BudgetApi.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITransactionService, TransactionServiceMock>();
builder.Services.AddSingleton<IReportService, ReportServiceMock>();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
  app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapReportEndpoints();
app.MapTransactionEndpoints();

app.Run();
