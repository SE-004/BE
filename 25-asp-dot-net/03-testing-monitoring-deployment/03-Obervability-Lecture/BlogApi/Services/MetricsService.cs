using System.Diagnostics.Metrics;

namespace BlogApi.Services;

public class MetricsService : IMetricsService
{
  private readonly Meter _meter;
  private readonly Counter<int> _userCreatedCounter;
  private readonly Counter<int> _postCreatedCounter;
  private readonly Counter<int> _loginAttemptCounter;

  public MetricsService()
  {
    _meter = new Meter("BlogApi", "1.0.0");

    _userCreatedCounter = _meter.CreateCounter<int>(
        "blog_api_users_created_total",
        "Count",
        "Total number of users created");

    _postCreatedCounter = _meter.CreateCounter<int>(
        "blog_api_posts_created_total",
        "Count",
        "Total number of posts created");

    _loginAttemptCounter = _meter.CreateCounter<int>(
        "blog_api_login_attempts_total",
        "Count",
        "Total number of login attempts");
  }

  public void RecordUserCreated() => _userCreatedCounter.Add(1);

  public void RecordPostCreated() => _postCreatedCounter.Add(1);

  public void RecordLoginAttempt(bool successful) =>
      _loginAttemptCounter.Add(1, new KeyValuePair<string, object?>("successful", successful));
}