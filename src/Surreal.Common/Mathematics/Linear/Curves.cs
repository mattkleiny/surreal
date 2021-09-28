using System;
using System.Numerics;
using JetBrains.Annotations;

namespace Surreal.Mathematics.Linear
{
  /// <summary>Describes a curve over a normalised parameter, T.</summary>
  public delegate float Curve(Normal t);

  /// <summary>Commonly used <see cref="Curve"/>s.</summary>
  public static class Curves
  {
    public static Curve Constant(float d) => _ => d;

    public static Curve Linear        { get; } = time => time;
    public static Curve InverseLinear { get; } = time => 1f - time;

    public static Curve PlanarX<T>(T curve) where T : IPlanarCurve => time => curve.SampleAt(time).X;
    public static Curve PlanarY<T>(T curve) where T : IPlanarCurve => time => curve.SampleAt(time).Y;
  }

  /// <summary>A planar curve that can be sampled at a point in 2-space.</summary>
  public interface IPlanarCurve
  {
    [Pure] Vector2 SampleAt(Normal t);
  }

  /// <summary>A quadratic bezier <see cref="IPlanarCurve"/>.</summary>
  public readonly record struct QuadraticBezierCurve(Vector2 Start, Vector2 A, Vector2 End) : IPlanarCurve
  {
    public Vector2 SampleAt(Normal t)
    {
      var x = MathF.Pow(1 - t, 2) * Start.X + 2 * (1 - t) * t * A.X + MathF.Pow(t, 2) * End.X;
      var y = MathF.Pow(1 - t, 2) * Start.Y + 2 * (1 - t) * t * A.Y + MathF.Pow(t, 2) * End.Y;

      return new Vector2(x, y);
    }
  }

  /// <summary>A cubic bezier <see cref="IPlanarCurve"/>.</summary>
  public readonly record struct CubicBezierCurve(Vector2 Start, Vector2 A, Vector2 B, Vector2 End) : IPlanarCurve
  {
    public Vector2 SampleAt(Normal t)
    {
      var t2 = t.Value * t.Value;
      var t3 = t.Value * t2;

      var oneT  = 1f - t;
      var oneT2 = oneT * oneT;
      var oneT3 = oneT2 * oneT;

      return Start * oneT3
             + A * 3f * oneT2 * t
             + B * 3f * oneT * t2
             + End * t3;
    }
  }
}
