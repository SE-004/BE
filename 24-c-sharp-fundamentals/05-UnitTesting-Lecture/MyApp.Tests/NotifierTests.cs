using MyApp;

public class NotifierTests
{
  [Fact]
  public void Publish_RaisesMessagePublishedEvent()
  {
    var notifier = new Notifier();
    string? received = null;
    notifier.MessagePublished += (s, msg) => received = msg;
    notifier.Publish("Hello");

    Assert.Equal("Hello", received);
  }

  [Fact]
  public void Unsubscribe_Handler_DoesNotReceiveEvent()
  {
    var notifier = new Notifier();
    string? received = null;
    EventHandler<string> handler = (s, msg) => received = msg;
    notifier.MessagePublished += handler;
    notifier.MessagePublished -= handler;

    notifier.Publish("Hello");

    Assert.Null(received);
  }
}