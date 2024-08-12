using Surreal.Diagnostics.Logging;

namespace Surreal.Fibers.Internal;

/// <summary>
/// .NET-compatible <see cref="SynchronizationContext"/> for task scheduling on Unity's player loop.
/// </summary>
internal sealed class FiberSynchronizationContext : SynchronizationContext
{
  private static readonly ILog Log = LogFactory.GetLog<FiberSynchronizationContext>();

  private readonly ConcurrentQueue<Continuation> _continuations = new();

  public override void Post(SendOrPostCallback callback, object? state)
  {
    _continuations.Enqueue(new Continuation(callback, state));
  }

  public override void Send(SendOrPostCallback callback, object? state)
  {
    _continuations.Enqueue(new Continuation(callback, state));
  }

  public void Execute()
  {
    while (_continuations.TryDequeue(out var continuation))
    {
      try
      {
        continuation.Callback.Invoke(continuation.State);
      }
      catch (Exception exception)
      {
        Log.Error(exception, "An error occurred whilst executing a continuation.");
      }
    }
  }

  /// <summary>
  /// An enqueued continuation to be later replayed.
  /// </summary>
  private readonly record struct Continuation(SendOrPostCallback Callback, object? State);
}