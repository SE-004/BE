namespace BlogApi.Services;

public class TimeService
{
  public string Now() => DateTime.Now.ToString("T");
}