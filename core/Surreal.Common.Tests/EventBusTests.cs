namespace Surreal.Common.Tests;

public class EventBusTests
{
  [Test]
  public void it_should_subscribe_and_publish_messages()
  {
    var eventBus = new EventBus();

    void OnEventReceived(ref TestEvent @event)
    {
      @event.IsReceived = true;
    }

    eventBus.Subscribe<TestEvent>(OnEventReceived);

    eventBus.Publish(new TestEvent()).IsReceived.Should().BeTrue();
    eventBus.Publish(new TestEvent()).IsReceived.Should().BeTrue();

    eventBus.Unsubscribe<TestEvent>(OnEventReceived);

    eventBus.Publish(new TestEvent()).IsReceived.Should().BeFalse();
    eventBus.Publish(new TestEvent()).IsReceived.Should().BeFalse();
  }

  [Test]
  public void it_should_discover_types_via_reflection()
  {
    var eventBus = new EventBus();
    var listener = new TestEventListener();

    eventBus.SubscribeAll(listener);

    eventBus.Publish(new TestEvent()).IsReceived.Should().BeTrue();
    eventBus.Publish(new TestEvent()).IsReceived.Should().BeTrue();

    eventBus.UnsubscribeAll(listener);

    eventBus.Publish(new TestEvent()).IsReceived.Should().BeFalse();
    eventBus.Publish(new TestEvent()).IsReceived.Should().BeFalse();
  }

  private struct TestEvent
  {
    public bool IsReceived { get; set; }
  }

  private sealed class TestEventListener
  {
    [EventListener]
    private void OnTestEvent(ref TestEvent message)
    {
      message.IsReceived = true;
    }
  }
}
