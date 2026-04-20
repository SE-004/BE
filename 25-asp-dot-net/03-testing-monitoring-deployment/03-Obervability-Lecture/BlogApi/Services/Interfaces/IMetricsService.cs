namespace BlogApi.Services;

public interface IMetricsService
{
  void RecordUserCreated();
  void RecordPostCreated();
  void RecordLoginAttempt(bool successful);
}