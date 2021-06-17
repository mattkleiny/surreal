﻿using System;
using Surreal.Collections.Pooling;
using Surreal.Mathematics.Timing;

namespace Surreal.Fibers.Promises {
  internal sealed class DelayPromise : Promise<Unit> {
    private static readonly Pool<DelayPromise> Pool = Pool<DelayPromise>.Shared;

    private readonly Action advanceCallback;
    private readonly Action returnCallback;

    private Timer   timer;
    private IClock? clock;

    public static DelayPromise Create(TimeSpan duration, bool useUnscaledTime) {
      var promise = Pool.CreateOrRent();

      // TODO: get top-level clock somehow?
      // promise.timer = new Timer(duration);
      // promise.clock = useUnscaledTime ? Clocks.Unscaled : Clocks.Standard;

      promise.Advance();

      return promise;
    }

    public DelayPromise() {
      advanceCallback = Advance;
      returnCallback  = () => Pool.Return(this);
    }

    private void Advance() {
      if (timer.Tick(clock!.DeltaTime)) {
        SetStatus(FiberTaskStatus.Succeeded);
        FiberScheduler.Schedule(returnCallback);
      }

      if (Status == FiberTaskStatus.Pending) {
        FiberScheduler.Schedule(advanceCallback);
      }
    }

    public override void OnReturn() {
      base.OnReturn();

      clock = default;
      timer = default;
    }
  }
}