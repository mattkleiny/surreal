using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Surreal.Collections;
using Surreal.Fibers;
using Surreal.Threading;

namespace Surreal;

public delegate void      MessageSubscriber<T>(ref T message);
public delegate FiberTask MessageSubscriberAsync<T>(ref T message);

/// <summary>Represents a listener for game <see cref="Message"/>s.</summary>
public interface IMessageListener
{
  void OnMessageReceived(Message message);
}

/// <summary>Indicates the associated method should subscribe to a message.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Method)]
public sealed class MessageSubscriberAttribute : Attribute
{
}

/// <summary>Represents a single message in the framework with support for value types and reference passing.</summary>
public readonly ref struct Message
{
  private static readonly LinkedList<IMessageListener>         Listeners        = new();
  private static readonly MultiDictionary<Type, Delegate>      Subscribers      = new();
  private static readonly MultiDictionary<Type, Delegate>      SubscribersAsync = new();
  private static readonly Dictionary<Type, SubscriberMethod[]> MethodCache      = new();
  private static readonly ReaderWriterLockSlim                 Lock             = new();

  public static void AddListener(IMessageListener listener)
  {
    using (Lock.ScopedWriteLock())
    {
      Listeners.AddLast(listener);
    }
  }

  public static void RemoveListener(IMessageListener listener)
  {
    using (Lock.ScopedWriteLock())
    {
      Listeners.Remove(listener);
    }
  }

  public static void Subscribe<T>(MessageSubscriber<T> subscriber)
  {
    using (Lock.ScopedWriteLock())
    {
      Subscribers.Add(typeof(T), subscriber);
    }
  }

  public static void Unsubscribe<T>(MessageSubscriber<T> subscriber)
  {
    using (Lock.ScopedWriteLock())
    {
      Subscribers.Remove(typeof(T), subscriber);
    }
  }

  public static void SubscribeAsync<T>(MessageSubscriberAsync<T> subscriber)
  {
    using (Lock.ScopedWriteLock())
    {
      SubscribersAsync.Add(typeof(T), subscriber);
    }
  }

  public static void UnsubscribeAsync<T>(MessageSubscriberAsync<T> subscriber)
  {
    using (Lock.ScopedWriteLock())
    {
      SubscribersAsync.Remove(typeof(T), subscriber);
    }
  }

  public static void SubscribeAll(object target)
  {
    using var _ = Lock.ScopedWriteLock();

    foreach (var method in GetOrCreateMethods(target))
    {
      var @delegate = method.Method.CreateDelegate(method.DelegateType, target);

      if (method.IsAsync)
      {
        SubscribersAsync.Add(method.MessageType, @delegate);
      }
      else
      {
        Subscribers.Add(method.MessageType, @delegate);
      }
    }
  }

  public static void UnsubscribeAll(object target)
  {
    using var _ = Lock.ScopedWriteLock();

    foreach (var method in GetOrCreateMethods(target))
    {
      var @delegate = method.Method.CreateDelegate(method.DelegateType, target);

      if (method.IsAsync)
      {
        SubscribersAsync.Remove(method.MessageType, @delegate);
      }
      else
      {
        Subscribers.Remove(method.MessageType, @delegate);
      }
    }
  }

  public static T Publish<T>(T data)
  {
    using var _ = Lock.ScopeReadLock();

    var message = Create(ref data);

    NotifyListeners(message);
    NotifySubscribers(ref data);
    NotifySubscribersAsync(ref data);

    return data;
  }

  public static FiberTask PublishAsync<T>(T data)
  {
    using var _ = Lock.ScopeReadLock();

    var message = Create(ref data);

    NotifyListeners(message);
    NotifySubscribers(ref data);

    return NotifySubscribersAsync(ref data);
  }

  public static async FiberTask<T> WaitForMessage<T>()
  {
    T   result     = default!;
    var isReceived = false;

    // ReSharper disable once ConvertToLocalFunction
    MessageSubscriber<T> subscriber = (ref T message) =>
    {
      result     = message;
      isReceived = true;
    };

    Subscribe(subscriber);

    try
    {
      while (!isReceived)
      {
        await FiberTask.Yield();
      }
    }
    finally
    {
      Unsubscribe(subscriber);
    }

    return result;
  }

  private static void NotifyListeners(Message message)
  {
    var handler = Listeners.First;

    while (handler != null)
    {
      handler.Value.OnMessageReceived(message);
      handler = handler.Next;
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
        var callback   = Unsafe.As<MessageSubscriber<T>>(subscriber);

        callback.Invoke(ref data);
      }
    }
  }

  private static FiberTask NotifySubscribersAsync<T>(ref T data)
  {
    if (SubscribersAsync.TryGetValues(typeof(T), out var subscribers))
    {
      var builder = FiberTask.WhenAll();

      // walk backwards so that subscribers might remove themselves
      for (var i = subscribers.Length - 1; i >= 0; i--)
      {
        var subscriber = subscribers[i];
        var callback   = Unsafe.As<MessageSubscriberAsync<T>>(subscriber);

        builder.AddTask(callback.Invoke(ref data));
      }

      return builder.Begin();
    }

    return FiberTask.CompletedTask;
  }

  private readonly unsafe void* data;

  private static unsafe Message Create<T>(ref T data)
  {
    var address = Unsafe.AsPointer(ref data);

    return new Message(address, typeof(T));
  }

  private unsafe Message(void* data, Type type)
  {
    this.data = data;
    Type      = type;
  }

  public Type Type { get; }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool Is<T>() => Type == typeof(T);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public unsafe ref T To<T>() => ref Unsafe.AsRef<T>(data);

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
        let isAsync = method.ReturnType == typeof(FiberTask)
        select new SubscriberMethod
        {
          Method      = method,
          MessageType = messageType,
          DelegateType = isAsync
            ? typeof(MessageSubscriberAsync<>).MakeGenericType(messageType)
            : typeof(MessageSubscriber<>).MakeGenericType(messageType),
          IsAsync = isAsync,
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
    public bool       IsAsync      { get; set; }
  }
}
