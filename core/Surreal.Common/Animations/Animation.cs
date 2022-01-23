using Surreal.Timing;

namespace Surreal.Animations;

/// <summary>A single animation that can be played over time.</summary>
public sealed record Animation
{
  public List<IAnimationTrack> Tracks { get; init; } = new();

  public TimeSpan TotalTime => TimeSpan.FromSeconds(EndTime - StartTime);

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

  public void Advance(DeltaTime deltaTime)
  {
    foreach (var track in Tracks)
    {
      track.Advance(deltaTime);
    }
  }

  public void Rewind(DeltaTime deltaTime)
  {
    foreach (var track in Tracks)
    {
      track.Rewind(deltaTime);
    }
  }
}
