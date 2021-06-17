using System;
using System.Numerics;
using JetBrains.Annotations;

namespace Surreal.Mathematics.Linear {
  public delegate float Curve(Normal time);

  public static class Curves {
    public static Curve Constant(float d) => _ => d;

    public static Curve Linear        { get; } = time => time;
    public static Curve InverseLinear { get; } = time => 1f - time;

    public static Curve PlanarX<T>(T curve) where T : IPlanarCurve => time => curve.SampleAt(time).X;
    public static Curve PlanarY<T>(T curve) where T : IPlanarCurve => time => curve.SampleAt(time).Y;
  }

  public interface IPlanarCurve {
    [Pure] Vector2 SampleAt(Normal t);
  }

  public readonly struct CubicBezierCurve : IEquatable<CubicBezierCurve>, IPlanarCurve {
    public readonly Vector2 StartPoint;
    public readonly Vector2 ControlPoint1;
    public readonly Vector2 ControlPoint2;
    public readonly Vector2 EndPoint;

    public CubicBezierCurve(Vector2 startPoint, Vector2 controlPoint1, Vector2 controlPoint2, Vector2 endPoint) {
      StartPoint    = startPoint;
      ControlPoint1 = controlPoint1;
      ControlPoint2 = controlPoint2;
      EndPoint      = endPoint;
    }

    public void Deconstruct(out Vector2 startPoint, out Vector2 controlPoint1, out Vector2 controlPoint2, out Vector2 endPoint) {
      startPoint    = StartPoint;
      controlPoint1 = ControlPoint1;
      controlPoint2 = ControlPoint2;
      endPoint      = EndPoint;
    }

    public Vector2 SampleAt(Normal t) {
      var t2 = t.Value * t.Value;
      var t3 = t.Value * t2;

      var oneT  = 1f - t;
      var oneT2 = oneT * oneT;
      var oneT3 = oneT2 * oneT;

      return StartPoint * oneT3
             + ControlPoint1 * 3f * oneT2 * t
             + ControlPoint2 * 3f * oneT * t2
             + EndPoint * t3;
    }

    public bool Equals(CubicBezierCurve other) =>
        StartPoint.Equals(other.StartPoint) &&
        ControlPoint1.Equals(other.ControlPoint1) &&
        ControlPoint2.Equals(other.ControlPoint2) &&
        EndPoint.Equals(other.EndPoint);

    public override bool Equals(object? obj) => obj is CubicBezierCurve other && Equals(other);
    public override int  GetHashCode()       => HashCode.Combine(StartPoint, ControlPoint1, ControlPoint2, EndPoint);

    public static bool operator ==(CubicBezierCurve left, CubicBezierCurve right) => left.Equals(right);
    public static bool operator !=(CubicBezierCurve left, CubicBezierCurve right) => !left.Equals(right);
  }

  public readonly struct QuadraticBezierCurve : IEquatable<QuadraticBezierCurve>, IPlanarCurve {
    public readonly Vector2 StartPoint;
    public readonly Vector2 ControlPoint;
    public readonly Vector2 EndPoint;

    public QuadraticBezierCurve(Vector2 startPoint, Vector2 controlPoint, Vector2 endPoint) {
      StartPoint   = startPoint;
      ControlPoint = controlPoint;
      EndPoint     = endPoint;
    }

    public void Deconstruct(out Vector2 startPoint, out Vector2 controlPoint, out Vector2 endPoint) {
      startPoint   = StartPoint;
      controlPoint = ControlPoint;
      endPoint     = EndPoint;
    }

    public Vector2 SampleAt(Normal t) {
      var x = MathF.Pow(1 - t, 2) * StartPoint.X + 2 * (1 - t) * t * ControlPoint.X + MathF.Pow(t, 2) * EndPoint.X;
      var y = MathF.Pow(1 - t, 2) * StartPoint.Y + 2 * (1 - t) * t * ControlPoint.Y + MathF.Pow(t, 2) * EndPoint.Y;

      return new Vector2(x, y);
    }

    public bool Equals(QuadraticBezierCurve other) =>
        StartPoint.Equals(other.StartPoint) &&
        ControlPoint.Equals(other.ControlPoint) &&
        EndPoint.Equals(other.EndPoint);

    public override bool Equals(object? obj) => obj is QuadraticBezierCurve other && Equals(other);
    public override int  GetHashCode()       => HashCode.Combine(StartPoint, ControlPoint, EndPoint);

    public static bool operator ==(QuadraticBezierCurve left, QuadraticBezierCurve right) => left.Equals(right);
    public static bool operator !=(QuadraticBezierCurve left, QuadraticBezierCurve right) => !left.Equals(right);
  }
}