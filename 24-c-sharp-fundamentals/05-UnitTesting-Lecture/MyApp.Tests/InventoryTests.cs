namespace MyApp.Tests;

using MyApp;

public class InventoryTests
{
  [Fact]
  public void AddAndGetAll_ContainsItems_InOrder()
  {
    var inv = new Inventory();
    var apple = "apple";
    var banana = "banana";
    inv.Add(apple);
    inv.Add(banana);

    var all = inv.GetAll();

    Assert.Equal(2, all.Count);
    // assert collection is empty
    // Assert.Empty( all);
    // assert 1 item in collection
    // Assert.Single(all)

    Assert.Contains(apple, all);
    Assert.DoesNotContain("cherry", all);
    Assert.Collection(all,
          first => Assert.Equal(apple, first),
          second => Assert.Equal(banana, second));
    foreach (var fruit in all)
    {
      Assert.IsType<string>(fruit);
    }
  }
}