namespace Surreal.Animations;

/// <summary>
/// Interpolates between two values.
/// </summary>
public delegate T Lerp<T>(T from, T to, float amount);

/// <summary>
/// An <see cref="IAnimationTrack"/> that interpolates a value over time.
/// </summary>
public sealed class LerpAnimationTrack<T>(IProperty<T> property) : KeyFrameAnimationTrack<T>(property)
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
