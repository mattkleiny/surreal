using System;
using System.Threading;

namespace Surreal.Fibers {
  internal sealed class FiberSynchronizationContext : SynchronizationContext {
    private readonly Fiber fiber;

    public FiberSynchronizationContext(Fiber fiber) {
      this.fiber = fiber;
    }

    public override SynchronizationContext CreateCopy() => this;

    public override void Post(SendOrPostCallback callback, object state) {
      if (fiber.State == FiberState.Completed) {
        throw new InvalidOperationException("The fiber has already completed but is awaiting continuations.");
      }

      fiber.Scheduler.Schedule(fiber, () => callback(state));
    }
  }
}