using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Surreal.Fibers;

/// <summary>A scheduler for <see cref="FiberTask"/>s.</summary>
[UsedImplicitly]
public static class FiberScheduler
{
  private static readonly ConcurrentQueue<Action> Callbacks = new();

  internal static void Schedule(Action callback)
  {
    Callbacks.Enqueue(callback);
  }

  public static void Tick()
  {
    while (Callbacks.TryDequeue(out var continuation))
    {
      try
      {
        continuation.Invoke();
      }
      catch (Exception exception)
      {
        Debug.Print("An error occurred whilst running callbacks: {0}", exception);
      }
    }

    FiberSynchronizationContext.Instance.Execute();
  }

  [SuppressMessage(
    "Critical Code Smell",
    "S927:Parameter names should match base declaration and other partial definitions",
    Justification = "Poorly named base class parameters in the standard library"
  )]
  private sealed class FiberSynchronizationContext : SynchronizationContext
  {
    public static FiberSynchronizationContext Instance { get; } = new();

    private readonly ConcurrentQueue<Continuation> continuations = new();

    public override void Post(SendOrPostCallback callback, object? state)
    {
      continuations.Enqueue(new Continuation(callback, state));
    }

    public override void Send(SendOrPostCallback callback, object? state)
    {
      continuations.Enqueue(new Continuation(callback, state));
    }

    public void Execute()
    {
      while (continuations.TryDequeue(out var continuation))
      {
        try
        {
          continuation.Callback.Invoke(continuation.State);
        }
        catch (Exception exception)
        {
          Debug.Print("An error occurred whilst running continuations: {0}", exception);
        }
      }
    }

    private readonly record struct Continuation(SendOrPostCallback Callback, object? State);
  }
}
