using System.Linq.Expressions;
using Surreal.Timing;
using Surreal.Utilities;

namespace Surreal.Animations;

/// <summary>
/// A key-frame in an <see cref="LerpAnimationTrack{T}"/>.
/// </summary>
public readonly record struct KeyFrame<T>(float Time, T Value);

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

/// <summary>
/// Base class for any animation track.
/// </summary>
public abstract class AnimationTrack<T>(IAnimationTrackDelegate<T> trackDelegate) : IAnimationTrack
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
    get => trackDelegate.GetValue();
    set => trackDelegate.SetValue(value);
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

/// <summary>
/// Interpolates between two values.
/// </summary>
public delegate T Lerp<T>(T from, T to, float amount);

/// <summary>
/// An <see cref="IAnimationTrack"/> that interpolates a value over time.
/// </summary>
public sealed class LerpAnimationTrack<T>(IAnimationTrackDelegate<T> trackDelegate) : AnimationTrack<T>(trackDelegate)
{
  /// <summary>
  /// The interpolation function to use for this track.
  /// </summary>
  public required Lerp<T> Lerp { get; init; }

  protected override T UpdateCurrentValue(float currentTime)
  {
    var (time1, value1) = PreviousKeyFrame;
    var (time2, value2) = NextKeyFrame;

    var normalizedTime = CurrentTime / (time2 - time1);

    return Lerp(value1, value2, normalizedTime);
  }
}

/// <summary>
/// A delegate for accessing a value, used by the <see cref="LerpAnimationTrack{T}"/>.
/// </summary>
public interface IAnimationTrackDelegate<T>
{
  /// <summary>
  /// Gets the current value.
  /// </summary>
  T GetValue();

  /// <summary>
  /// Sets the current value.
  /// </summary>
  /// <param name="value"></param>
  void SetValue(T value);
}

/// <summary>
/// Static factory for <see cref="IAnimationTrackDelegate{T}"/>s.
/// </summary>
public abstract class AnimationTrackDelegate
{
  /// <summary>
  /// Creates a new <see cref="IAnimationTrackDelegate{T}"/> from the given expression.
  /// </summary>
  public static IAnimationTrackDelegate<T> Create<TRoot, T>(TRoot root, Expression<Func<TRoot, T?>> expression)
    where TRoot : class
  {
    if (!expression.TryResolveProperty(out var propertyInfo))
    {
      throw new InvalidOperationException("The given expression doesn't represent a valid property");
    }

    if (propertyInfo.GetMethod == null || propertyInfo.SetMethod == null)
    {
      throw new InvalidOperationException($"The given property {propertyInfo} doesn't have a valid getter and setter");
    }

    var getter = propertyInfo.GetMethod.CreateDelegate<Func<T>>(root);
    var setter = propertyInfo.SetMethod.CreateDelegate<Action<T>>(root);

    return new AnimationTrackDelegateImpl<T>(getter, setter);
  }

  /// <summary>
  /// A <see cref="IAnimationTrackDelegate{T}"/> that uses a getter and setter to access a value.
  /// </summary>
  private sealed class AnimationTrackDelegateImpl<T>(Func<T> getter, Action<T> setter) : AnimationTrackDelegate, IAnimationTrackDelegate<T>
  {
    /// <inheritdoc/>
    public T GetValue() => getter();

    /// <inheritdoc/>
    public void SetValue(T value) => setter(value);
  }
}
