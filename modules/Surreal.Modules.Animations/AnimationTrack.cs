using System.Linq.Expressions;
using Surreal.Timing;
using Surreal.Utilities;

namespace Surreal;

/// <summary>A key-frame in an <see cref="AnimationTrack{T}"/>.</summary>
public readonly record struct KeyFrame<T>(float Time, T Value);

/// <summary>A single track that can be advanced over time.</summary>
public interface IAnimationTrack
{
  float StartTime { get; }
  float EndTime   { get; }

  void Advance(DeltaTime deltaTime);
  void Rewind(DeltaTime deltaTime);
}

/// <summary>A delegate for accessing a value, used by the <see cref="AnimationTrack{T}"/>.</summary>
public interface IAnimationTrackDelegate<T>
{
  T    GetValue();
  void SetValue(T value);
}

/// <summary>Static factory for <see cref="AnimationTrack"/>s.</summary>
public static class AnimationTrack
{
  public static AnimationTrack<T> Create<TRoot, T>(TRoot root, Expression<Func<TRoot, T?>> expression)
    where TRoot : class
  {
    return new AnimationTrack<T>(CreateDelegate(root, expression));
  }

  private static IAnimationTrackDelegate<T> CreateDelegate<TRoot, T>(TRoot root, Expression<Func<TRoot, T?>> expression)
    where TRoot : class
  {
    if (!expression.TryResolveProperty(out var propertyInfo))
    {
      throw new InvalidOperationException("The given expression doesn't represent a valid property");
    }

    if (propertyInfo.GetMethod == null || propertyInfo.SetMethod == null)
    {
      throw new InvalidOperationException($"The given1 property {propertyInfo} doesn't have a valid getter and setter");
    }

    var getter = propertyInfo.GetMethod.CreateDelegate<Func<T>>(root);
    var setter = propertyInfo.SetMethod.CreateDelegate<Action<T>>(root);

    return new AnimationTrackDelegate<T>(getter, setter);
  }

  private sealed record AnimationTrackDelegate<T>(Func<T> Getter, Action<T> Setter) : IAnimationTrackDelegate<T>
  {
    public T    GetValue()        => Getter();
    public void SetValue(T value) => Setter(value);
  }
}

/// <summary>A single <see cref="IAnimationTrack"/> for a single target type, <see cref="T"/>.</summary>
public sealed record AnimationTrack<T>(IAnimationTrackDelegate<T> Delegate) : IAnimationTrack
{
  public Interpolator<T> Interpolator { get; init; } = Interpolators.Default<T>();
  public KeyFrame<T>[]   KeyFrames    { get; init; } = Array.Empty<KeyFrame<T>>();

  public float StartTime => KeyFrames.Length > 0 ? KeyFrames[0].Time : 0f;
  public float EndTime   => KeyFrames.Length > 0 ? KeyFrames[^1].Time : 0f;

  public float CurrentTime { get; private set; }

  public T CurrentValue
  {
    get => Delegate.GetValue();
    set => Delegate.SetValue(value);
  }

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

  public void Advance(DeltaTime deltaTime)
  {
    CurrentTime += deltaTime;

    UpdateKeyFrame();
  }

  public void Rewind(DeltaTime deltaTime)
  {
    CurrentTime -= deltaTime;

    UpdateKeyFrame();
  }

  private void UpdateKeyFrame()
  {
    // TODO: fix this up

    var (time1, value1) = PreviousKeyFrame;
    var (time2, value2) = NextKeyFrame;

    var normalizedTime = CurrentTime / (time2 - time1);

    CurrentValue = Interpolator(value1, value2, normalizedTime);
  }
}
