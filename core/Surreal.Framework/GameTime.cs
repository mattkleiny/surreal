using Surreal.Timing;

namespace Surreal;

/// <summary>Encapsulates the frame-by-frame timing information for a game.</summary>
[DebuggerDisplay("{DeltaTime} since last frame")]
public readonly record struct GameTime(DeltaTime DeltaTime, TimeSpan TotalTime, bool IsRunningSlowly);
