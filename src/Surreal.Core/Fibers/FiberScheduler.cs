using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using Surreal.Collections;
using Surreal.Diagnostics.Profiling;

namespace Surreal.Fibers {
  public sealed class FiberScheduler {
    private static readonly IProfiler Profiler = ProfilerFactory.GetProfiler<FiberScheduler>();

    private readonly ActionQueue queue = new ActionQueue();

    public bool   PropagateExceptions { get; set; } = true;
    public Fiber? CurrentFiber        { get; private set; }

    public void Run() {
      using var _ = Profiler.Track(nameof(Run));

      queue.Execute();
    }

    internal void Schedule(Fiber fiber, Action task) {
      queue.Enqueue(() => {
        var previousFiber       = CurrentFiber;
        var previousSyncContext = SynchronizationContext.Current;

        try {
          CurrentFiber = fiber;
          SynchronizationContext.SetSynchronizationContext(CurrentFiber.SynchronizationContext);

          task.Invoke();
        }
        finally {
          CurrentFiber = previousFiber;
          SynchronizationContext.SetSynchronizationContext(previousSyncContext);
        }
      });
    }

    internal void NotifyException(Exception exception) {
      if (PropagateExceptions) {
        ExceptionDispatchInfo.Capture(exception).Throw();
      }
    }
  }
}