using Surreal.Timing;

namespace Surreal.Animations;

/// <summary>
/// A key-frame in an <see cref="KeyFrameAnimationTrack{T}"/>.
/// </summary>
public readonly record struct KeyFrame<T>(float Time, T Value);

/// <summary>
/// Base class for any animation track.
/// </summary>
public abstract class KeyFrameAnimationTrack<T>(IProperty<T> property) : IAnimationTrack
{
  /// <summary>
  /// The key-frames that make up this track.
  /// </summary>
  public List<KeyFrame<T>> KeyFrames { get; init; } = [];

  /// <inheritdoc/>
  public float StartTime => KeyFrames.Count > 0 ? KeyFrames[0].Time : 0f;

  /// <inheritdoc/>
  public float EndTime => KeyFrames.Count > 0 ? KeyFrames[^1].Time : 0f;

  /// <summary>
  /// The current time of this track.
  /// </summary>
  public float CurrentTime { get; private set; }

  /// <summary>
  /// The current value of this track.
  /// </summary>
  public T CurrentValue
  {
    get => property.Value;
    set => property.Value = value;
  }

  /// <summary>
  /// The previous key-frame, relative to <see cref="CurrentTime"/>.
  /// </summary>
  public KeyFrame<T> PreviousKeyFrame
  {
    get
    {
      if (KeyFrames.Count == 0)
        return default;

      var bestKeyFrame = KeyFrames[0];

      foreach (var frame in KeyFrames)
      {
        if (frame.Time > bestKeyFrame.Time && frame.Time < CurrentTime)
        {
          bestKeyFrame = frame;
        }
      }

      return bestKeyFrame;
    }
  }

  /// <summary>
  /// The next key-frame, relative to <see cref="CurrentTime"/>.
  /// </summary>
  public KeyFrame<T> NextKeyFrame
  {
    get
    {
      if (KeyFrames.Count == 0)
        return default;

      var bestKeyFrame = KeyFrames[^1];

      foreach (var frame in KeyFrames)
      {
        if (frame.Time < bestKeyFrame.Time && frame.Time > CurrentTime)
        {
          bestKeyFrame = frame;
        }
      }

      return bestKeyFrame;
    }
  }

  /// <inheritdoc/>
  public void Advance(DeltaTime deltaTime)
  {
    CurrentTime += deltaTime;
    CurrentValue = UpdateCurrentValue(CurrentTime);
  }

  /// <inheritdoc/>
  public void Rewind(DeltaTime deltaTime)
  {
    CurrentTime -= deltaTime;
    CurrentValue = UpdateCurrentValue(CurrentTime);
  }

  /// <summary>
  /// Updates the current value based on the current time.
  /// </summary>
  protected abstract T UpdateCurrentValue(float currentTime);
}
