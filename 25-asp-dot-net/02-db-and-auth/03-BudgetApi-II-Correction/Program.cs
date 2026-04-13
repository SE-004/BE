
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using BudgetApi.Api.Endpoints;
using BudgetApi.Application.Services;
using BudgetApi.Application.Interfaces;
using BudgetApi.Infrastructure;
using BudgetApi.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ITransactionService, TransactionServiceEf>();
builder.Services.AddScoped<IReportService, ReportServiceEf>();
builder.Services.AddScoped<DbSeeder>();
// builder.Services.AddSingleton<ITransactionService, TransactionServiceMock>();
// builder.Services.AddSingleton<IReportService, ReportServiceMock>();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
  app.MapScalarApiReference();
  using (var scope = app.Services.CreateScope())
  {
    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    await seeder.SeedAsync();
  }
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapReportEndpoints();
app.MapTransactionEndpoints();

app.Run();
