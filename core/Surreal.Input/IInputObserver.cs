namespace Surreal.Input;

/// <summary>
/// Represents an event that can be published by an input device.
/// </summary>
public interface IInputEvent;

/// <summary>
/// Represents an observable set of <see cref="IInputEvent"/>s.
/// </summary>
public interface IInputObservable
{
  static IInputObservable Null { get; } = new NullInputObservable();

  /// <summary>
  /// Creates a new <see cref="IInputObservable"/> that combines multiple observables.
  /// </summary>
  static IInputObservable Combine(IEnumerable<IInputObservable> observables) => new CompositeObservable(observables);

  void Subscribe(IInputObserver observer);
  void Unsubscribe(IInputObserver observer);

  /// <summary>
  /// A composite <see cref="IInputObservable"/> that combines multiple observables.
  /// </summary>
  private sealed class CompositeObservable(IEnumerable<IInputObservable> observables) : IInputObservable
  {
    public void Subscribe(IInputObserver observer)
    {
      foreach (var observable in observables)
      {
        observable.Subscribe(observer);
      }
    }

    public void Unsubscribe(IInputObserver observer)
    {
      foreach (var observable in observables)
      {
        observable.Unsubscribe(observer);
      }
    }
  }

  /// <summary>
  /// A no-op <see cref="IInputObservable"/>.
  /// </summary>
  [ExcludeFromCodeCoverage]
  private sealed class NullInputObservable : IInputObservable
  {
    public void Subscribe(IInputObserver observer)
    {
    }

    public void Unsubscribe(IInputObserver observer)
    {
    }
  }
}

/// <summary>
/// An observer for <see cref="IInputEvent"/>s.
/// </summary>
public interface IInputObserver
{
  void OnNext<TEvent>(TEvent @event)
    where TEvent : IInputEvent;
}

/// <summary>
/// A subject for <see cref="IInputEvent"/>s.
/// </summary>
public sealed class InputEventSubject : IInputObservable
{
  private readonly List<IInputObserver> _observers = [];

  /// <inheritdoc/>
  public void Subscribe(IInputObserver observer)
  {
    _observers.Add(observer);
  }

  /// <inheritdoc/>
  public void Unsubscribe(IInputObserver observer)
  {
    _observers.Remove(observer);
  }

  /// <summary>
  /// Notifies all observers that a new event is available.
  /// </summary>
  public void NotifyNext<TEvent>(TEvent @event)
    where TEvent : IInputEvent
  {
    // walk backwards so that we can remove observers as we go
    for (var i = _observers.Count - 1; i >= 0; i--)
    {
      _observers[i].OnNext(@event);
    }
  }
}
