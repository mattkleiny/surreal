using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Surreal.Collections;

namespace Surreal;

/// <summary>A subscriber for <see cref="Message"/>s.</summary>
public delegate void MessageSubscriber<T>(ref T message);

/// <summary>Indicates the associated method should subscribe to a message.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Method)]
public sealed class MessageSubscriberAttribute : Attribute
{
}

/// <summary>A simple message bus that allows communication amongst disconnected clients.</summary>
public static class Message
{
  private static readonly MultiDictionary<Type, Delegate> Subscribers = new();
  private static readonly ConcurrentDictionary<Type, SubscriberMethod[]> MethodCache = new();

  public static T Publish<T>(T data)
  {
    NotifySubscribers(ref data);

    return data;
  }

  public static void Subscribe<T>(MessageSubscriber<T> subscriber)
  {
    lock (Subscribers)
    {
      Subscribers.Add(typeof(T), subscriber);
    }
  }

  public static void Unsubscribe<T>(MessageSubscriber<T> subscriber)
  {
    lock (Subscribers)
    {
      Subscribers.Remove(typeof(T), subscriber);
    }
  }

  public static void SubscribeAll(object target)
  {
    foreach (var method in DiscoverSubscriberMethods(target))
    {
      var @delegate = method.Method.CreateDelegate(method.DelegateType, target);

      lock (Subscribers)
      {
        Subscribers.Add(method.MessageType, @delegate);
      }
    }
  }

  public static void UnsubscribeAll(object target)
  {
    foreach (var method in DiscoverSubscriberMethods(target))
    {
      var @delegate = method.Method.CreateDelegate(method.DelegateType, target);

      lock (Subscribers)
      {
        Subscribers.Remove(method.MessageType, @delegate);
      }
    }
  }

  private static void NotifySubscribers<T>(ref T data)
  {
    ReadOnlySlice<Delegate> subscribers;

    // optimistically grab all available subscribers before notifcation
    lock (Subscribers)
    {
      if (!Subscribers.TryGetValues(typeof(T), out subscribers))
      {
        return;
      }
    }

    // walk backwards so that subscribers might remove themselves
    for (var i = subscribers.Length - 1; i >= 0; i--)
    {
      var subscriber = subscribers[i];
      var callback = Unsafe.As<MessageSubscriber<T>>(subscriber);

      callback.Invoke(ref data);
    }
  }

  [RequiresUnreferencedCode("Discovers methods via reflection")]
  private static SubscriberMethod[] DiscoverSubscriberMethods(object target)
  {
    static IEnumerable<MethodInfo> DiscoverAllMethods(Type type)
    {
      IEnumerable<MethodInfo> methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

      if (type.BaseType != null)
      {
        methods = methods.Concat(DiscoverAllMethods(type.BaseType));
      }

      return methods;
    }

    static SubscriberMethod[] DiscoverWithReflection(Type type)
    {
      var results =
        from method in DiscoverAllMethods(type)
        where method.GetCustomAttribute<MessageSubscriberAttribute>() != null
        let parameters = method.GetParameters()
        where parameters.Length == 1 && parameters[0].ParameterType.IsByRef
        let messageType = parameters[0].ParameterType.GetElementType()
        select new SubscriberMethod
        {
          Method       = method,
          MessageType  = messageType,
          DelegateType = typeof(MessageSubscriber<>).MakeGenericType(messageType),
        };

      return results.ToArray();
    }

    return MethodCache.GetOrAdd(target.GetType(), DiscoverWithReflection);
  }

  /// <summary>Represents a reflected subscriber method.</summary>
  private record struct SubscriberMethod(MethodInfo Method, Type MessageType, Type DelegateType);
}
