using Surreal.Collections;
using Surreal.Timing;

namespace Surreal.Fibers.Promises;

internal sealed class DelayPromise : Promise<Unit>
{
  private static readonly Pool<DelayPromise> Pool = Pool<DelayPromise>.Shared;

  private readonly Action advanceCallback;
  private readonly Action returnCallback;

  private IClock? clock;
  private IntervalTimer   timer;

  public static DelayPromise Create(IClock clock, TimeSpan duration)
  {
    var promise = Pool.CreateOrRent();

    promise.clock = clock;
    promise.timer = new IntervalTimer(duration);

    promise.Advance();

    return promise;
  }

  public DelayPromise()
  {
    advanceCallback = Advance;
    returnCallback  = () => Pool.Return(this);
  }

  private void Advance()
  {
    if (timer.Tick(clock!.DeltaTime))
    {
      SetStatus(FiberTaskStatus.Succeeded);
      FiberScheduler.Schedule(returnCallback);
    }

    if (Status == FiberTaskStatus.Pending)
    {
      FiberScheduler.Schedule(advanceCallback);
    }
  }

  public override void OnReturn()
  {
    base.OnReturn();

    clock = default;
    timer = default;
  }
}
