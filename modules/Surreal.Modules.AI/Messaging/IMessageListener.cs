namespace Surreal.Messaging;

/// <summary>A listener for <see cref="Message"/>s.</summary>
public interface IMessageListener
{
  void OnMessageReceived(Message message);
}

/// <summary>Static extensions for <see cref="IMessageListener"/>s.</summary>
public static class MessageListenerExtensions
{
  public static T SendMessage<T>(this IMessageListener listener, T payload)
  {
    var message = Message.Create(ref payload);

    listener.OnMessageReceived(message);

    return payload;
  }
}
