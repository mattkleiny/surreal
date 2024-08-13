using Surreal.Timing;

namespace Surreal.Animations;

/// <summary>
/// A single track that can be advanced over time.
/// </summary>
public interface IAnimationTrack
{
  float StartTime { get; }
  float EndTime { get; }

  void Advance(DeltaTime deltaTime);
  void Rewind(DeltaTime deltaTime);
}
