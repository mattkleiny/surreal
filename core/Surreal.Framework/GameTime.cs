using System.Diagnostics;
using Surreal.Timing;

namespace Surreal;

/// <summary>Encapsulates the frame-by-frame timing information for a game.</summary>
[DebuggerDisplay("{DeltaTime} since last frame")]
public readonly ref struct GameTime
{
  public readonly DeltaTime DeltaTime;
  public readonly TimeSpan  TotalTime;
  public readonly bool      IsRunningSlowly;

  public GameTime(DeltaTime deltaTime, TimeSpan totalTime, bool isRunningSlowly)
  {
    DeltaTime       = deltaTime;
    TotalTime       = totalTime;
    IsRunningSlowly = isRunningSlowly;
  }
}
