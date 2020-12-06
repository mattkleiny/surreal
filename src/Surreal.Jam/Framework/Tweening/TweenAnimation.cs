using System;
using Surreal.Mathematics.Curves;
using Surreal.Mathematics.Timing;

namespace Surreal.Framework.Tweening {
  public readonly struct TweenAnimation : IEquatable<TweenAnimation> {
    public static TweenAnimation Default => new(1.Seconds(), AnimationCurves.Linear);

    public readonly TimeSpan       Duration;
    public readonly AnimationCurve Curve;

    public TweenAnimation(TimeSpan duration, AnimationCurve curve) {
      Duration = duration;
      Curve    = curve;
    }

    public          bool Equals(TweenAnimation other) => Duration.Equals(other.Duration) && Curve.Equals(other.Curve);
    public override bool Equals(object? obj)          => obj is TweenAnimation other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Duration, Curve);

    public static bool operator ==(TweenAnimation left, TweenAnimation right) => left.Equals(right);
    public static bool operator !=(TweenAnimation left, TweenAnimation right) => !left.Equals(right);
  }
}