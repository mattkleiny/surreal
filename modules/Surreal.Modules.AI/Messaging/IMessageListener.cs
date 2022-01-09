using System.Runtime.CompilerServices;

namespace Surreal.Messaging;

/// <summary>A listener for <see cref="Message"/>s.</summary>
public interface IMessageListener
{
  void OnMessageReceived(Message message);
}

/// <summary>Static extensions for <see cref="IMessageListener"/>s.</summary>
public static class MessageListenerExtensions
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T SendMessage<T>(this IMessageListener listener, T payload)
  {
    listener.OnMessageReceived(Message.Create(ref payload));

    return payload;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T SendMessage<T>(this IMessageListener listener, ref T payload)
  {
    listener.OnMessageReceived(Message.Create(ref payload));

    return payload;
  }
}
