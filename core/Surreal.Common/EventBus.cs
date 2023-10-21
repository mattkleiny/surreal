using Surreal.Collections;
using Surreal.Utilities;

namespace Surreal;

/// <summary>
/// A listener for events.
/// </summary>
public delegate void EventListener<TEvent>(ref TEvent @event);

/// <summary>
/// Indicates the associated method should be an <see cref="EventListener{TEvent}"/>.
/// </summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Method)]
public sealed class EventListenerAttribute : Attribute;

/// <summary>
/// A mechanism for publishing and subscribing to events.
/// </summary>
public interface IEventBus
{
  /// <summary>
  /// Publishes an event to all subscribers.
  /// </summary>
  T Publish<T>(T message);

  /// <summary>
  /// Subscribes to events of a given type.
  /// </summary>
  void Subscribe<T>(EventListener<T> handler);

  /// <summary>
  /// Unsubscribes from events of a given type.
  /// </summary>
  void Unsubscribe<T>(EventListener<T> handler);

  /// <summary>
  /// Subscribes all <see cref="EventListenerAttribute"/> annotated methods on the given object.
  /// </summary>
  void SubscribeAll(object target);

  /// <summary>
  /// Unsubscribes all <see cref="EventListenerAttribute"/> annotated methods on the given object.
  /// </summary>
  void UnsubscribeAll(object target);
}

/// <summary>
/// The default implementation of <see cref="IEventBus"/>.
/// </summary>
public sealed class EventBus(IEventBus? parent = null) : IEventBus
{
  private readonly MultiDictionary<Type, Delegate> _listenerByType = new();

  public T Publish<T>(T message)
  {
    if (_listenerByType.TryGetValues(typeof(T), out var listeners))
    {
      // walk backwards so that we can remove listeners as we go
      for (var i = listeners.Length - 1; i >= 0; i--)
      {
        var listener = Unsafe.As<EventListener<T>>(listeners[i]);

        listener.Invoke(ref message);
      }
    }

    if (parent != null)
    {
      message = parent.Publish(message);
    }

    return message;
  }

  public void Subscribe<TEvent>(EventListener<TEvent> handler)
  {
    _listenerByType.Add(typeof(TEvent), handler);
  }

  public void Unsubscribe<TEvent>(EventListener<TEvent> handler)
  {
    _listenerByType.Remove(typeof(TEvent), handler);
  }

  public void SubscribeAll(object target)
  {
    foreach (var method in SubscriberMethod.DiscoverAll(target))
    {
      var @delegate = method.Method.CreateDelegate(method.DelegateType, target);

      _listenerByType.Add(method.MessageType, @delegate);
    }
  }

  public void UnsubscribeAll(object target)
  {
    foreach (var method in SubscriberMethod.DiscoverAll(target))
    {
      var @delegate = method.Method.CreateDelegate(method.DelegateType, target);

      _listenerByType.Remove(method.MessageType, @delegate);
    }
  }

  /// <summary>
  /// Represents a reflected subscriber method.
  /// </summary>
  private readonly record struct SubscriberMethod
  {
    public MethodInfo Method { get; init; }
    public Type MessageType { get; init; }
    public Type DelegateType { get; init; }

    private static readonly ConcurrentDictionary<Type, SubscriberMethod[]> DiscoveryCache = new();

    /// <summary>
    /// Discovers all methods on the given target object that are decorated with <see cref="EventListenerAttribute"/>.
    /// </summary>
    public static IEnumerable<SubscriberMethod> DiscoverAll(object target)
    {
      var type = target.GetType();

      if (!DiscoveryCache.TryGetValue(type, out var methods))
      {
        var results =
          from method in type.GetHierarchyMethods()
          where method.GetCustomAttribute<EventListenerAttribute>() != null
          let parameters = method.GetParameters()
          where parameters.Length == 1 && parameters[0].ParameterType.IsByRef
          let messageType = parameters[0].ParameterType.GetElementType()
          select new SubscriberMethod
          {
            Method = method,
            MessageType = messageType,
            DelegateType = typeof(EventListener<>).MakeGenericType(messageType),
          };

        DiscoveryCache[type] = methods = results.ToArray();
      }

      return methods;
    }
  }
}
