
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using System.Text;
using Serilog;
using Serilog.Events;

using BudgetApi.Api.Endpoints;
using BudgetApi.Application.Services;
using BudgetApi.Application.Interfaces;
using BudgetApi.Infrastructure;
using BudgetApi.Infrastructure.Data;
using BudgetApi.Models;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.EnvironmentName != "Testing")
{
  builder.Services.AddDbContext<ApplicationDbContext>(options =>
      options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddIdentityCore<User>()
.AddRoles<IdentityRole<Guid>>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ClockSkew = TimeSpan.Zero
      };
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITransactionService, TransactionServiceEf>();
builder.Services.AddScoped<IReportService, ReportServiceEf>();
builder.Services.AddScoped<DbSeeder>();

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

builder.Host.UseSerilog((context, configuration) =>
{
  configuration
      .ReadFrom.Configuration(context.Configuration)
      .WriteTo.Console(outputTemplate:
          "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
      .WriteTo.File("logs/blog-api-.txt",
          rollingInterval: RollingInterval.Day,
          outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
      .Enrich.FromLogContext()
      .Enrich.WithProperty("Application", "BlogApi");
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>() // Check database connectivity
    .AddCheck("self", () => HealthCheckResult.Healthy("API is running"))
    .AddCheck<CustomHealthCheck>("custom-check"); // Custom business logic check
// Register custom health check
builder.Services.AddScoped<CustomHealthCheck>();
builder.Services.AddSingleton<IMetricsService, MetricsService>();

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

app.UseSerilogRequestLogging(options =>
{
  options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
  options.GetLevel = (httpContext, elapsed, ex) => ex != null ? LogEventLevel.Error : LogEventLevel.Information;
  options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
  {
    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value ?? "unknown");
    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
    diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.FirstOrDefault() ?? "unknown");
  };
});

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapReportEndpoints();
app.MapTransactionEndpoints();
app.MapHealthEndpoints();

app.Run();
