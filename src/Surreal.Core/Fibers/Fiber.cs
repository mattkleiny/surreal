using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Surreal.Diagnostics.Logging;

namespace Surreal.Fibers {
  // TODO: implement no-allocation FiberTask helpers and async method builder.

  public sealed class Fiber {
    private static readonly ILog Log = LogFactory.GetLog<Fiber>();

    private static long nextFiberId;

    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    public static Fiber Start(FiberScheduler scheduler, Func<Task> method) {
      return Start(scheduler, _ => method());
    }

    public static Fiber Start(FiberScheduler scheduler, Func<CancellationToken, Task> method) {
      var fiber = new Fiber(scheduler);

      fiber.Execute(method);

      return fiber;
    }

    private Fiber(FiberScheduler scheduler) {
      Id = Interlocked.Increment(ref nextFiberId);

      Scheduler              = scheduler;
      SynchronizationContext = new FiberSynchronizationContext(this);
    }

    public long              Id                { get; }
    public FiberScheduler    Scheduler         { get; }
    public Exception?        Exception         { get; private set; }
    public FiberState        State             { get; private set; }
    public CancellationToken CancellationToken => cancellationTokenSource.Token;

    public bool IsCompleted => State == FiberState.Completed ||
                               State == FiberState.Faulted ||
                               State == FiberState.Cancelled;

    public bool IsFaulted => State == FiberState.Faulted;

    internal FiberSynchronizationContext SynchronizationContext { get; }

    public void Cancel() {
      cancellationTokenSource.Cancel();
    }

    public void PropagateException() {
      if (Exception != null) {
        ExceptionDispatchInfo.Capture(Exception).Throw();
      }
    }

    private void Execute(Func<CancellationToken, Task> method) {
      Transition(FiberState.New, FiberState.Running);

      Scheduler.Schedule(this, async () => {
        try {
          await method(CancellationToken);

          Transition(FiberState.Running, FiberState.Completed);
        } catch (OperationCanceledException) {
          Transition(FiberState.Running, FiberState.Cancelled);
        } catch (Exception exception) {
          Log.Error($"An unexpected error occurred whilst executing a fiber: {exception}");

          Exception = exception;
          Scheduler.NotifyException(exception);

          Transition(FiberState.Running, FiberState.Faulted);
        }
      });
    }

    private void Transition(FiberState required, FiberState desired) {
      if (!State.Equals(required)) {
        throw new InvalidOperationException($"Expected to be in the {required} state, instead was in the {State} state.");
      }

      State = desired;
    }
  }
}