using System;
using System.Diagnostics;
using Surreal.Mathematics.Timing;

namespace Surreal.Framework {
  [DebuggerDisplay("{DeltaTime} since last frame")]
  public readonly struct GameTime {
    public readonly DeltaTime DeltaTime;
    public readonly TimeSpan  TotalTime;
    public readonly bool      IsRunningSlowly;

    public GameTime(DeltaTime deltaTime, TimeSpan totalTime, bool isRunningSlowly) {
      DeltaTime       = deltaTime;
      TotalTime       = totalTime;
      IsRunningSlowly = isRunningSlowly;
    }
  }
}