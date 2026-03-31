namespace MyApp.Tests;

using MyApp;

public class StringToolsTests
{
  [Fact]
  public void Reverse_ReturnsExpected()
  {
    Assert.Equal("cba", StringTools.Reverse("abc"));
    Assert.NotEqual("abc", StringTools.Reverse("abc"));

    // boolean checks
    // Assert.True(true /*expected*/);
    // Assert.False(false /*expected*/);
  }

  [Theory]
  [InlineData("racecar", true)]
  [InlineData("RaceCar", true)]
  [InlineData("hello", false)]
  public void IsPalindrome_Works(string input, bool expected)
  {
    Assert.Equal(expected, StringTools.IsPalindrome(input));
    Assert.IsType<bool>(StringTools.IsPalindrome(input));
  }
}