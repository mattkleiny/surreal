﻿namespace Surreal;

public class MessagingTests
{
  [Test]
  public void it_should_send_message_to_listener()
  {
    var listener = new TestListener();

    listener.SendMessage("This is a test!");
  }

  [Test]
  public void it_should_dispatch_to_registered_subscribers()
  {
    Message.SubscribeAll(this);

    try
    {
      Message.Publish("This is a test!");
    }
    finally
    {
      Message.UnsubscribeAll(this);
    }
  }

  [MessageSubscriber]
  private void OnMessageReceived(ref string value)
  {
    Assert.AreEqual("This is a test!", value);
  }

  private sealed class TestListener : IMessageListener
  {
    void IMessageListener.OnMessageReceived(Message message)
    {
      Assert.IsTrue(message.Is<string>());
      Assert.AreEqual("This is a test!", message.Cast<string>());
    }
  }
}