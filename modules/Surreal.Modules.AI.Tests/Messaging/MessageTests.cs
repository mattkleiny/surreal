using System.Runtime.CompilerServices;

namespace Surreal.Messaging;

public class MessageTests
{
  [Test]
  public void it_should_create_a_valid_message()
  {
    var payload = new TestMessage();
    var message = Message.Create(ref payload);

    Assert.IsTrue(message.Is<TestMessage>());
    Assert.IsFalse(Unsafe.IsNullRef(ref message.Cast<TestMessage>()));
  }

  [Test]
  public void it_should_packed_payload_to_listener()
  {
    var listener = new TestListener();
    var result   = listener.SendMessage(new TestMessage());

    Assert.AreEqual(8, result.Width);
    Assert.AreEqual(7, result.Height);
  }

  private record struct TestMessage()
  {
    public int Width  { get; set; } = 16;
    public int Height { get; set; } = 9;
  }

  private sealed class TestListener : IMessageListener
  {
    void IMessageListener.OnMessageReceived(Message message)
    {
      Assert.IsTrue(message.Is<TestMessage>());

      ref var payload = ref message.Cast<TestMessage>();

      Assert.AreEqual(16, payload.Width);
      Assert.AreEqual(9, payload.Height);

      payload.Width  = 8;
      payload.Height = 7;
    }
  }
}
