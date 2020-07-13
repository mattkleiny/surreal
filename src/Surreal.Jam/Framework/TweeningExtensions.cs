using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Surreal.Framework.Parameters;
using Surreal.Graphics;
using Surreal.Mathematics.Curves;
using Surreal.Timing;

namespace Surreal.Framework {
  public delegate T Interpolator<T>(T a, T b, float t);

  public static class TweeningExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task TweenOverTime(this FloatParameter parameter, IClock clock, float a, float b, TimeSpan duration, AnimationCurve curve, CancellationToken cancellationToken = default)
      => TweenOverTime(parameter, clock, Maths.Lerp, a, b, duration, curve, cancellationToken);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task TweenOverTime(this IntParameter parameter, IClock clock, int a, int b, TimeSpan duration, AnimationCurve curve, CancellationToken cancellationToken = default)
      => TweenOverTime(parameter, clock, Maths.Lerp, a, b, duration, curve, cancellationToken);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task TweenOverTime(this ColorParameter parameter, IClock clock, Color a, Color b, TimeSpan duration, AnimationCurve curve, CancellationToken cancellationToken = default)
      => TweenOverTime(parameter, clock, Color.Lerp, a, b, duration, curve, cancellationToken);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task TweenOverTime(this Vector2Parameter parameter, IClock clock, Vector2 a, Vector2 b, TimeSpan duration, AnimationCurve curve, CancellationToken cancellationToken = default)
      => TweenOverTime(parameter, clock, Vector2.Lerp, a, b, duration, curve, cancellationToken);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task TweenOverTime(this Vector3Parameter parameter, IClock clock, Vector3 a, Vector3 b, TimeSpan duration, AnimationCurve curve, CancellationToken cancellationToken = default)
      => TweenOverTime(parameter, clock, Vector3.Lerp, a, b, duration, curve, cancellationToken);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task TweenOverTime(this Vector4Parameter parameter, IClock clock, Vector4 a, Vector4 b, TimeSpan duration, AnimationCurve curve, CancellationToken cancellationToken = default)
      => TweenOverTime(parameter, clock, Vector4.Lerp, a, b, duration, curve, cancellationToken);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task TweenOverTime<T>(this Parameter<T> parameter, IClock clock, Interpolator<T> evaluator, T a, T b, TimeSpan duration, AnimationCurve curve,
        CancellationToken cancellationToken = default)
      => clock.EvaluateOverTime(duration, time => parameter.Value = evaluator(a, b, curve(time.NormalizedTime)), cancellationToken);
  }
}