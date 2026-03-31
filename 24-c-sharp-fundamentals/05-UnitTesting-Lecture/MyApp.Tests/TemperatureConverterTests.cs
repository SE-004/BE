namespace MyApp.Tests;

using MyApp;

public class TemperatureConverterTests
{
  [Theory]
  [InlineData(0, 32)]
  [InlineData(100, 212)]
  public void CelsiusToFahrenheit_KnownPoints(double c, double f)
      => Assert.Equal(f, TemperatureConverter.CelsiusToFahrenheit(c), precision: 5);

  [Fact]
  public void CelsiusToFahrenheit_InRange()
  {
    var f = TemperatureConverter.CelsiusToFahrenheit(20);
    Assert.InRange(f, 67.9, 68.1);
  }

  [Fact]
  public void StringAsserts_Examples()
  {
    var msg = "Hello, world!";
    Assert.StartsWith("Hello", msg);
    Assert.EndsWith("!", msg);
    Assert.NotNull(msg);
    // Assert.Null(null);
  }
}