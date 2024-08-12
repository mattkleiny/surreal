using Surreal.Collections.Pooling;
using Surreal.Timing;

namespace Surreal.Fibers.Promises;

/// <summary>
/// A <see cref="Promise{T}"/> with deferred evaluation.
/// </summary>
internal sealed class DelayPromise : Promise<Void>
{
  private static readonly Pool<DelayPromise> Pool = new(() => new());

  private readonly Action _advanceCallback;
  private readonly Action _returnCallback;

  private IntervalTimer _timer;
  private CancellationToken _cancellationToken;

  public static DelayPromise Create(TimeSpan duration, CancellationToken cancellationToken)
  {
    var promise = Pool.CreateOrRent();

    promise._timer = new IntervalTimer(duration);
    promise._cancellationToken = cancellationToken;

    promise.Advance();

    return promise;
  }

  private DelayPromise()
  {
    _advanceCallback = Advance;
    _returnCallback = () => Pool.Return(this);
  }

  private void Advance()
  {
    var scheduler = FiberScheduler.Current;

    if (_cancellationToken.IsCancellationRequested)
    {
      SetStatus(FiberTaskStatus.Canceled);
      scheduler.Schedule(_returnCallback);
    }
    else if (_timer.Tick(scheduler.DeltaTime))
    {
      SetStatus(FiberTaskStatus.Succeeded);
      scheduler.Schedule(_returnCallback);
    }
    else if (Status == FiberTaskStatus.Pending)
    {
      scheduler.Schedule(_advanceCallback);
    }
    else if (Status == FiberTaskStatus.Canceled)
    {
      _returnCallback();
    }
  }

  public override void OnReturn()
  {
    base.OnReturn();

    _timer = default;
    _cancellationToken = default;
  }
}