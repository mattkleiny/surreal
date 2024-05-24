using Surreal.Timing;

namespace Surreal.Animations;

/// <summary>
/// A single animation that can be played over time.
/// </summary>
public sealed record Animation
{
  /// <summary>
  /// The tracks that make up this animation.
  /// </summary>
  public List<IAnimationTrack> Tracks { get; init; } = [];

  /// <summary>
  /// The total time of this animation.
  /// </summary>
  public TimeSpan TotalTime => TimeSpan.FromSeconds(EndTime - StartTime);

  /// <summary>
  /// The start time of this animation.
  /// </summary>
  public float StartTime
  {
    get
    {
      var bestTime = float.MaxValue;

      foreach (var track in Tracks)
      {
        if (track.StartTime < bestTime)
        {
          bestTime = track.StartTime;
        }
      }

      return bestTime;
    }
  }

  /// <summary>
  /// The end time of this animation.
  /// </summary>
  public float EndTime
  {
    get
    {
      var bestTime = 0f;

      foreach (var track in Tracks)
      {
        if (track.EndTime > bestTime)
        {
          bestTime = track.EndTime;
        }
      }

      return bestTime;
    }
  }

  /// <summary>
  /// Advances this animation by the given delta time.
  /// </summary>
  public void Advance(DeltaTime deltaTime)
  {
    foreach (var track in Tracks)
    {
      track.Advance(deltaTime);
    }
  }

  /// <summary>
  /// Rewinds this animation by the given delta time.
  /// </summary>
  public void Rewind(DeltaTime deltaTime)
  {
    foreach (var track in Tracks)
    {
      track.Rewind(deltaTime);
    }
  }
}
