﻿namespace Surreal;

public class MessagingTests
{
  [Test]
  public void it_should_notify_subscribers()
  {
    Message.SubscribeAll(this);
    try
    {
      Message.Publish(new TestMessage());
    }
    finally
    {
      Message.UnsubscribeAll(this);
    }

    Assert.Fail();
  }

  [MessageSubscriber]
  private void OnMessageReceived(ref TestMessage message)
  {
    message.Width.Should().Be(16);
    message.Height.Should().Be(9);

    Assert.Pass();
  }

  private sealed record TestMessage
  {
    public int Width { get; } = 16;
    public int Height { get; } = 9;
  }
}
