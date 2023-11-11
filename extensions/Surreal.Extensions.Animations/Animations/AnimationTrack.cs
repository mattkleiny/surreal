using System.Linq.Expressions;
using Surreal.Maths;
using Surreal.Timing;
using Surreal.Utilities;

namespace Surreal.Animations;

/// <summary>
/// Interpolates between two values.
/// </summary>
public delegate T InterpolationFunction<T>(T from, T to, float amount);

/// <summary>
/// A key-frame in an <see cref="AnimationTrack{T}"/>.
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
/// A delegate for accessing a value, used by the <see cref="AnimationTrack{T}"/>.
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
/// Static factory for <see cref="AnimationTrack"/>s.
/// </summary>
public static class AnimationTrack
{
  public static AnimationTrack<int> Create<TRoot>(TRoot root, Expression<Func<TRoot, int>> expression)
    where TRoot : class => Create(root, MathE.Lerp, expression);

  public static AnimationTrack<float> Create<TRoot>(TRoot root, Expression<Func<TRoot, float>> expression)
    where TRoot : class => Create(root, MathE.Lerp, expression);

  public static AnimationTrack<double> Create<TRoot>(TRoot root, Expression<Func<TRoot, double>> expression)
    where TRoot : class => Create(root, MathE.Lerp, expression);

  public static AnimationTrack<T> Create<TRoot, T>(TRoot root, Expression<Func<TRoot, T?>> expression)
    where TRoot : class
    where T :IInterpolated<T>
  {
    return Create(root, T.Lerp, expression);
  }

  public static AnimationTrack<T> Create<TRoot, T>(TRoot root, InterpolationFunction<T> interpolator, Expression<Func<TRoot, T?>> expression)
    where TRoot : class
  {
    return new AnimationTrack<T>(CreateDelegate(root, expression)) { Interpolator = interpolator };
  }

  private static AnimationTrackDelegate<T> CreateDelegate<TRoot, T>(TRoot root, Expression<Func<TRoot, T?>> expression)
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

    return new AnimationTrackDelegate<T>(getter, setter);
  }

  /// <summary>
  /// A <see cref="IAnimationTrackDelegate{T}"/> that uses a getter and setter to access a value.
  /// </summary>
  private sealed class AnimationTrackDelegate<T>(Func<T> getter, Action<T> setter) : IAnimationTrackDelegate<T>
  {
    /// <inheritdoc/>
    public T GetValue() => getter();

    /// <inheritdoc/>
    public void SetValue(T value) => setter(value);
  }
}

/// <summary>
/// A single <see cref="IAnimationTrack"/> for a single target type, <see cref="T"/>.
/// </summary>
public sealed record AnimationTrack<T>(IAnimationTrackDelegate<T> Delegate) : IAnimationTrack
{
  /// <summary>
  /// The interpolation function to use for this track.
  /// </summary>
  public required InterpolationFunction<T> Interpolator { get; init; }

  /// <summary>
  /// The key-frames that make up this track.
  /// </summary>
  public KeyFrame<T>[] KeyFrames { get; init; } = Array.Empty<KeyFrame<T>>();

  /// <inheritdoc/>
  public float StartTime => KeyFrames.Length > 0 ? KeyFrames[0].Time : 0f;

  /// <inheritdoc/>
  public float EndTime => KeyFrames.Length > 0 ? KeyFrames[^1].Time : 0f;

  /// <summary>
  /// The current time of this track.
  /// </summary>
  public float CurrentTime { get; private set; }

  /// <summary>
  /// The current value of this track.
  /// </summary>
  public T CurrentValue
  {
    get => Delegate.GetValue();
    set => Delegate.SetValue(value);
  }

  /// <summary>
  /// The previous key-frame, relative to <see cref="CurrentTime"/>.
  /// </summary>
  public KeyFrame<T> PreviousKeyFrame
  {
    get
    {
      if (KeyFrames.Length == 0)
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
      if (KeyFrames.Length == 0)
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

    UpdateKeyFrame();
  }

  /// <inheritdoc/>
  public void Rewind(DeltaTime deltaTime)
  {
    CurrentTime -= deltaTime;

    UpdateKeyFrame();
  }

  /// <summary>
  /// Updates the current value based on the current time.
  /// </summary>
  private void UpdateKeyFrame()
  {
    // TODO: fix this up

    var (time1, value1) = PreviousKeyFrame;
    var (time2, value2) = NextKeyFrame;

    var normalizedTime = CurrentTime / (time2 - time1);

    CurrentValue = Interpolator(value1, value2, normalizedTime);
  }
}
