using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Surreal.Framework.Parameters;
using Surreal.Graphics;
using Surreal.Mathematics;
using Surreal.Mathematics.Timing;

namespace Surreal.Framework.Tweening {
  public delegate T Interpolator<T>(T a, T b, float t);

  public static class TweenExtensions {
    public static Task TweenOverTime(this FloatParameter parameter, IClock clock, float a, float b, TweenAnimation animation)
      => TweenOverTime(parameter, clock, Maths.Lerp, a, b, animation);

    public static Task TweenOverTime(this IntParameter parameter, IClock clock, int a, int b, TweenAnimation animation)
      => TweenOverTime(parameter, clock, Maths.Lerp, a, b, animation);

    public static Task TweenOverTime(this ColorParameter parameter, IClock clock, Color a, Color b, TweenAnimation animation)
      => TweenOverTime(parameter, clock, Color.Lerp, a, b, animation);

    public static Task TweenOverTime(this Vector2Parameter parameter, IClock clock, Vector2 a, Vector2 b, TweenAnimation animation)
      => TweenOverTime(parameter, clock, Vector2.Lerp, a, b, animation);

    public static Task TweenOverTime(this Vector3Parameter parameter, IClock clock, Vector3 a, Vector3 b, TweenAnimation animation)
      => TweenOverTime(parameter, clock, Vector3.Lerp, a, b, animation);

    public static Task TweenOverTime(this Vector4Parameter parameter, IClock clock, Vector4 a, Vector4 b, TweenAnimation animation)
      => TweenOverTime(parameter, clock, Vector4.Lerp, a, b, animation);

    public static Task TweenOverTime(this AngleParameter parameter, IClock clock, Angle a, Angle b, TweenAnimation animation)
      => TweenOverTime(parameter, clock, Angle.Lerp, a, b, animation);

    public static Task TweenOverTime<T>(
        this Parameter<T> parameter,
        Interpolator<T> interpolator,
        T a, T b,
        TweenAnimation animation,
        CancellationToken cancellationToken = default) {
      return TweenOverTime(parameter, Game.Current.Clock, interpolator, a, b, animation, cancellationToken);
    }

    public static Task TweenOverTime<T>(
        this Parameter<T> parameter,
        IClock clock,
        Interpolator<T> interpolator,
        T a, T b,
        TweenAnimation animation,
        CancellationToken cancellationToken = default) {
      return clock.EvaluateOverTime(animation.Duration, time => parameter.Value = interpolator(a, b, animation.Curve(time.Normal)), cancellationToken);
    }
  }
}