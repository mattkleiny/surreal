using Surreal.Collections;
using Surreal.Input.Collections;

namespace Surreal.Input;

/// <summary>
/// Manages dispatching <see cref="TAction"/>s from <see cref="IInputEvent"/>s.
/// </summary>
public sealed class InputManager<TAction> : IInputObserver, IDisposable
  where TAction : notnull
{
  private readonly VariableMultiDictionary<TAction> _actions = new();
  private readonly MultiDictionary<TAction, Action> _handlers = new();
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
  /// Adds a handler for the given action.
  /// </summary>
  public void AddHandler(TAction action, Action callback)
  {
    _handlers.Add(action, callback);
  }

  /// <summary>
  /// Removes a handler from the given action.
  /// </summary>
  public void RemoveHandler(TAction action, Action callback)
  {
    _handlers.Remove(action, callback);
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

      foreach (var handler in _handlers[action])
      {
        handler.Invoke();
      }
    }
  }

  /// <inheritdoc/>
  public void Dispose()
  {
    _observable.Unsubscribe(this);

    _actions.Clear();
    _handlers.Clear();
    _actionQueue.Clear();
  }

  /// <inheritdoc/>
  void IInputObserver.OnNext<TEvent>(TEvent @event)
  {
    ProcessEvent(@event);
  }
}
