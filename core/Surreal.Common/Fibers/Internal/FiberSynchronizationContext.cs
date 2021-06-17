using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Surreal.Fibers.Internal {
  internal sealed class FiberSynchronizationContext : SynchronizationContext {
    private readonly ConcurrentQueue<Continuation> continuations = new();

    public override void Post(SendOrPostCallback callback, object? state) {
      base.Post(callback, state);

      continuations.Enqueue(new Continuation(callback, state));
    }

    public override void Send(SendOrPostCallback callback, object? state) {
      base.Send(callback, state);

      continuations.Enqueue(new Continuation(callback, state));
    }

    public void Execute() {
      while (continuations.TryDequeue(out var continuation)) {
        try {
          continuation.Callback.Invoke(continuation.State);
        }
        catch (Exception exception) {
          // TODO: log the exception
        }
      }
    }

    public void Clear() {
      continuations.Clear();
    }

    private readonly struct Continuation {
      public readonly SendOrPostCallback Callback;
      public readonly object?            State;

      public Continuation(SendOrPostCallback callback, object? state) {
        Callback = callback;
        State    = state;
      }
    }
  }
}