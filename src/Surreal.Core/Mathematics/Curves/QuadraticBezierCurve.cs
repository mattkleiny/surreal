using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Surreal.Mathematics.Curves {
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
      var x = System.MathF.Pow(1 - t, 2) * StartPoint.X + 2 * (1 - t) * t * ControlPoint.X + System.MathF.Pow(t, 2) * EndPoint.X;
      var y = System.MathF.Pow(1 - t, 2) * StartPoint.Y + 2 * (1 - t) * t * ControlPoint.Y + System.MathF.Pow(t, 2) * EndPoint.Y;

      return new Vector2(x, y);
    }

    public Vector2 SampleDerivativeAt(Normal t) {
      var (c0, c1, c2) = GetDerivativeCoefficients(t);

      return StartPoint * c0 + ControlPoint * c1 + EndPoint * c2;
    }

    public CubicBezierCurve ToCubicBezierCurve() => new CubicBezierCurve(
        startPoint: StartPoint,
        controlPoint1: StartPoint + ControlPoint * 2f / 3f,
        controlPoint2: EndPoint   + ControlPoint * 2f / 3f,
        endPoint: EndPoint
    );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (float c0, float c1, float c2) GetDerivativeCoefficients(Normal t) {
      return (c0: 2f * t - 2f, c1: -4f * t + 2f, c2: 2f * t);
    }

    public bool Equals(QuadraticBezierCurve other) =>
        StartPoint.Equals(other.StartPoint)     &&
        ControlPoint.Equals(other.ControlPoint) &&
        EndPoint.Equals(other.EndPoint);

    public override bool Equals(object obj) => obj is QuadraticBezierCurve other && Equals(other);
    public override int  GetHashCode()      => HashCode.Combine(StartPoint, ControlPoint, EndPoint);

    public static bool operator ==(QuadraticBezierCurve left, QuadraticBezierCurve right) => left.Equals(right);
    public static bool operator !=(QuadraticBezierCurve left, QuadraticBezierCurve right) => !left.Equals(right);
  }
}