using System.Diagnostics.Metrics;
using BudgetApi.Application.Interfaces;


namespace BudgetApi.Application.Services;

public class MetricsService : IMetricsService
{
  private readonly Meter _meter;
  private readonly Counter<int> _userRegisteredCounter;
  private readonly Counter<int> _transactionCreatedCounter;
  private readonly Counter<int> _loginAttemptCounter;

  public MetricsService()
  {
    _meter = new Meter("BudgetApi", "1.0.0");

    _userRegisteredCounter = _meter.CreateCounter<int>(
        "budget_api_users_registered_total",
        "Count",
        "Total number of users registered");

    _transactionCreatedCounter = _meter.CreateCounter<int>(
        "budget_api_transactions_created_total",
        "Count",
        "Total number of transactions created");

    _loginAttemptCounter = _meter.CreateCounter<int>(
        "budget_api_login_attempts_total",
        "Count",
        "Total number of login attempts");
  }

  public void RecordUserRegistered() => _userRegisteredCounter.Add(1);

  public void RecordTransactionCreated() => _transactionCreatedCounter.Add(1);

  public void RecordLoginAttempt(bool successful) =>
      _loginAttemptCounter.Add(1, new KeyValuePair<string, object?>("successful", successful));
}