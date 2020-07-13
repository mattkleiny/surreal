using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Surreal.Mathematics.Curves {
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
      var oneT2 = oneT  * oneT;
      var oneT3 = oneT2 * oneT;

      return StartPoint                   * oneT3
             + ControlPoint1 * 3f * oneT2 * t
             + ControlPoint2 * 3f * oneT  * t2
             + EndPoint                   * t3;
    }

    public Vector2 SampleDerivativeAt(Normal t) {
      var (c0, c1, c2, c3) = GetDerivativeCoefficients(t);

      return StartPoint * c0 + ControlPoint1 * c1 + ControlPoint2 * c2 + EndPoint * c3;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (float c0, float c1, float c2, float c3) GetDerivativeCoefficients(Normal t) {
      var t2 = t.Value * t.Value;

      return (
          c0: -3f * t2 + 6f  * t - 3f,
          c1: 9f  * t2 - 12f * t + 3f,
          c2: -9f * t2           + 6f * t,
          c3: 3f * t2
      );
    }

    public bool Equals(CubicBezierCurve other) =>
        StartPoint.Equals(other.StartPoint)       &&
        ControlPoint1.Equals(other.ControlPoint1) &&
        ControlPoint2.Equals(other.ControlPoint2) &&
        EndPoint.Equals(other.EndPoint);

    public override bool Equals(object obj) => obj is CubicBezierCurve other && Equals(other);
    public override int  GetHashCode()      => HashCode.Combine(StartPoint, ControlPoint1, ControlPoint2, EndPoint);

    public static bool operator ==(CubicBezierCurve left, CubicBezierCurve right) => left.Equals(right);
    public static bool operator !=(CubicBezierCurve left, CubicBezierCurve right) => !left.Equals(right);
  }
}