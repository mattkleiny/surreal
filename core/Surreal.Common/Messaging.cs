using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Surreal.Collections;
using Surreal.Utilities;

namespace Surreal;

/// <summary>Represents a listener for game <see cref="Message"/>s.</summary>
public interface IMessageListener
{
  void OnMessageReceived(Message message);
}

/// <summary>Static extensions for <see cref="IMessageListener"/>s.</summary>
public static class MessageListenerExtensions
{
  public static T SendMessage<T>(this IMessageListener listener, T payload)
  {
    listener.OnMessageReceived(Message.Create(ref payload));

    return payload;
  }

  public static T SendMessage<T>(this IMessageListener listener, ref T payload)
  {
    listener.OnMessageReceived(Message.Create(ref payload));

    return payload;
  }
}

/// <summary>A listener for <see cref="Message"/>s.</summary>
public delegate void MessageSubscriber<T>(ref T message);

/// <summary>Indicates the associated method should subscribe to a message.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Method)]
public sealed class MessageSubscriberAttribute : Attribute
{
}

/// <summary>Represents a single message in the framework with support for value types and reference passing.</summary>
public readonly ref struct Message
{
  private static readonly LinkedList<IMessageListener> Listeners = new();
  private static readonly MultiDictionary<Type, Delegate> Subscribers = new();
  private static readonly Dictionary<Type, SubscriberMethod[]> MethodCache = new();

  public static void AddListener(IMessageListener listener) => Listeners.AddLast(listener);
  public static void RemoveListener(IMessageListener listener) => Listeners.Remove(listener);

  public static void Subscribe<T>(MessageSubscriber<T> subscriber) => Subscribers.Add(typeof(T), subscriber);
  public static void Unsubscribe<T>(MessageSubscriber<T> subscriber) => Subscribers.Remove(typeof(T), subscriber);

  public static void SubscribeAll(object target)
  {
    foreach (var method in GetOrCreateMethods(target))
    {
      var @delegate = method.Method.CreateDelegate(method.DelegateType, target);

      Subscribers.Add(method.MessageType, @delegate);
    }
  }

  public static void UnsubscribeAll(object target)
  {
    foreach (var method in GetOrCreateMethods(target))
    {
      var @delegate = method.Method.CreateDelegate(method.DelegateType, target);

      Subscribers.Remove(method.MessageType, @delegate);
    }
  }

  public static T Publish<T>(T data)
  {
    var message = Create(ref data);

    NotifyListeners(message);
    NotifySubscribers(ref data);

    return data;
  }

  private static void NotifyListeners(Message message)
  {
    for (var listener = Listeners.First; listener != null; listener = listener.Next)
    {
      listener.Value.OnMessageReceived(message);
    }
  }

  private static void NotifySubscribers<T>(ref T data)
  {
    if (Subscribers.TryGetValues(typeof(T), out var subscribers))
    {
      // walk backwards so that subscribers might remove themselves
      for (var i = subscribers.Length - 1; i >= 0; i--)
      {
        var subscriber = subscribers[i];
        var callback = Unsafe.As<MessageSubscriber<T>>(subscriber);

        callback.Invoke(ref data);
      }
    }
  }

  private readonly unsafe void* data;

  public static unsafe Message Create<T>(ref T data)
  {
    var address = Unsafe.AsPointer(ref data);

    return new Message(address, typeof(T));
  }

  private unsafe Message(void* data, Type type)
  {
    this.data = data;
    Type = type;
  }

  public Type Type { get; }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool Is<T>()
  {
    return Type == typeof(T);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public unsafe ref T Cast<T>()
  {
    return ref Unsafe.AsRef<T>(data);
  }

  private static SubscriberMethod[] GetOrCreateMethods(object target)
  {
    var type = target.GetType();

    if (!MethodCache.TryGetValue(type, out var methods))
    {
      var results =
        from method in type.GetFlattenedInstanceMethods()
        where method.GetCustomAttribute<MessageSubscriberAttribute>() != null
        let parameters = method.GetParameters()
        where parameters.Length == 1 && parameters[0].ParameterType.IsByRef
        let messageType = parameters[0].ParameterType.GetElementType()
        select new SubscriberMethod
        {
          Method = method,
          MessageType = messageType,
          DelegateType = typeof(MessageSubscriber<>).MakeGenericType(messageType),
        };

      MethodCache[type] = methods = results.ToArray();
    }

    return methods;
  }

  /// <summary>Represents a reflected subscriber method.</summary>
  private struct SubscriberMethod
  {
    public MethodInfo Method       { get; set; }
    public Type       MessageType  { get; set; }
    public Type       DelegateType { get; set; }
  }
}
