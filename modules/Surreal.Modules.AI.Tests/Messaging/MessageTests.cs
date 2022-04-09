using System.Runtime.CompilerServices;

namespace Surreal.Messaging;

public class MessageTests
{
  [Test]
  public void it_should_create_a_valid_message()
  {
    var payload = new TestMessage();
    var message = Message.Create(ref payload);

    message.Is<TestMessage>().Should().BeTrue();
    Unsafe.IsNullRef(ref message.Cast<TestMessage>()).Should().BeFalse();
  }

  [Test]
  public void it_should_packed_payload_to_listener()
  {
    var listener = new TestListener();
    var result = listener.SendMessage(new TestMessage());

    result.Width.Should().Be(8);
    result.Height.Should().Be(7);
  }

  [Test]
  public void it_should_notify_subscribers()
  {
    Message.SubscribeAll(this);
    Message.Publish("Hello, World");

    Assert.Fail();
  }

  [MessageSubscriber]
  private void OnMessageReceived(ref string message)
  {
    Assert.Pass();
  }

  private record struct TestMessage
  {
    public int Width  { get; set; } = 16;
    public int Height { get; set; } = 9;
  }

  private sealed class TestListener : IMessageListener
  {
    void IMessageListener.OnMessageReceived(Message message)
    {
      message.Is<TestMessage>().Should().BeTrue();

      ref var payload = ref message.Cast<TestMessage>();

      payload.Width.Should().Be(16);
      payload.Height.Should().Be(9);

      payload.Width = 8;
      payload.Height = 7;
    }
  }
}
