using System;

namespace Surreal.Fibers {
  /// <summary>Permits interruption and resumption of a single logical <see cref="FiberTask"/>.</summary>
  public struct FiberTaskCell : IDisposable {
    private FiberTask task;

    public void Restart(Func<FiberTask> factory) {
      Restart(factory());
    }

    public void Restart(FiberTask task) {
      Cancel();
      this.task = task;
    }

    public void Cancel()  => task.Cancel();
    public void Dispose() => Cancel();

    public FiberTaskAwaiter GetAwaiter() => task.GetAwaiter();
  }
}