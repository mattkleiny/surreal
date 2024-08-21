using Surreal.Collections;

namespace Surreal.Input;

/// <summary>
/// Manages dispatching <see cref="TAction"/>s from <see cref="IInputEvent"/>s.
/// </summary>
public sealed class InputManager<TAction> : IInputObserver, IDisposable
{
  private readonly VariableMultiDictionary<TAction> _actions = new();
  private readonly Queue<TAction> _actionQueue = new();
  private readonly IInputObservable _observable;

  public InputManager(IInputObservable observable)
  {
    _observable = observable;

    observable.Subscribe(this);
  }

  /// <summary>
  /// Raised when an <see cref="TAction"/> is triggered.
  /// </summary>
  public event Action<TAction>? ActionTriggered;

  /// <summary>
  /// Binds the given action to the given event.
  /// </summary>
  public void BindAction<TEvent>(TAction action, TEvent @event)
    where TEvent : IInputEvent
  {
    _actions.Add(@event, action);
  }

  /// <summary>
  /// Unbinds the given action from the given event.
  /// </summary>
  public void UnbindAction<TEvent>(TAction action, TEvent @event)
    where TEvent : IInputEvent
  {
    _actions.Remove(@event, action);
  }

  /// <summary>
  /// Processes the given event.
  /// </summary>
  public void ProcessEvent<TEvent>(TEvent @event)
    where TEvent : IInputEvent
  {
    // find all actions mapped to the given input event
    if (_actions.TryGetValues(@event, out var actions))
    {
      foreach (var action in actions)
      {
        _actionQueue.Enqueue(action);
      }
    }
  }

  /// <summary>
  /// Flushes all queued actions.
  /// </summary>
  public void Flush()
  {
    while (_actionQueue.TryDequeue(out var action))
    {
      ActionTriggered?.Invoke(action);
    }
  }

  /// <inheritdoc/>
  public void Dispose()
  {
    _observable.Unsubscribe(this);
  }

  /// <inheritdoc/>
  void IInputObserver.OnNext<TEvent>(TEvent @event)
  {
    ProcessEvent(@event);
  }
}
