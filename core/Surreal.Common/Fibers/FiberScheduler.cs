using Surreal.Diagnostics.Logging;
using Surreal.Fibers.Internal;
using Surreal.Timing;

namespace Surreal.Fibers;

/// <summary>
/// Permits scheduling runtime operations for <see cref="FiberTask"/>s.
/// </summary>
public sealed class FiberScheduler
{
  private static readonly ILog Log = LogFactory.GetLog<FiberScheduler>();

  private readonly ConcurrentQueue<Action> _continuations = new();
  private readonly Queue<Action> _executions = new();
  private readonly FiberSynchronizationContext _syncContext = new();

  /// <summary>
  /// The currently active <see cref="FiberScheduler"/>.
  /// </summary>
  public static FiberScheduler Current { get; } = new();

  /// <summary>
  /// The current delta time.
  /// </summary>
  public DeltaTime DeltaTime { get; private set; }

  /// <summary>
  /// Installs the <see cref="FiberScheduler"/> as the current <see cref="SynchronizationContext"/>.
  /// </summary>
  private void Install()
  {
    SynchronizationContext.SetSynchronizationContext(_syncContext);
  }

  /// <summary>
  /// Schedules an action to be executed on the next tick of the scheduler.
  /// </summary>
  public void Schedule(Action callback)
  {
    _continuations.Enqueue(callback);
  }

  /// <summary>
  /// Advances all pending operations on the scheduler.
  /// </summary>
  public void Tick(DeltaTime deltaTime)
  {
    DeltaTime = deltaTime;

    // transfer from continuations to executions
    while (_continuations.TryDequeue(out var callback))
    {
      _executions.Enqueue(callback);
    }

    while (_executions.TryDequeue(out var callback))
    {
      try
      {
        callback.Invoke();
      }
      catch (Exception exception)
      {
        Log.Error(exception, "An unhandled exception occurred while executing a fiber task.");
      }
    }

    _syncContext.Execute();
  }
}