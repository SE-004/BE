namespace BudgetApi.Application.Interfaces;

public interface IMetricsService
{
  void RecordUserRegistered();
  void RecordTransactionCreated();
  void RecordLoginAttempt(bool successful);
}