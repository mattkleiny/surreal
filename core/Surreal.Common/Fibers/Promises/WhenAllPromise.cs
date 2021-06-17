using System;
using Surreal.Collections.Pooling;

namespace Surreal.Fibers.Promises {
  internal sealed class WhenAllPromise : Promise<Unit> {
    private static readonly Pool<WhenAllPromise> Pool = Pool<WhenAllPromise>.Shared;

    private readonly Action returnCallback;

    private bool isStarted;
    private int  totalCount;
    private int  completedCount;

    public static WhenAllPromise Create() {
      return Pool.CreateOrRent();
    }

    public WhenAllPromise() {
      returnCallback = () => Pool.Return(this);
    }

    public void AddTask(FiberTask task) {
      totalCount += 1;

      task.GetAwaiter().OnCompleted(() => {
        completedCount += 1;

        if (isStarted) {
          CheckForCompletion();
        }
      });
    }

    private void CheckForCompletion() {
      if (completedCount >= totalCount) {
        SetStatus(FiberTaskStatus.Succeeded);
        FiberScheduler.Schedule(returnCallback);
      }
    }

    public void Advance() {
      isStarted = true;

      CheckForCompletion();
    }

    public override void OnReturn() {
      base.OnReturn();

      isStarted      = false;
      totalCount     = 0;
      completedCount = 0;
    }
  }
}