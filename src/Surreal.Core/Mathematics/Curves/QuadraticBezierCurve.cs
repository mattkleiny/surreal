using System;
using System.Numerics;

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
      var x = MathF.Pow(1 - t, 2) * StartPoint.X + 2 * (1 - t) * t * ControlPoint.X + MathF.Pow(t, 2) * EndPoint.X;
      var y = MathF.Pow(1 - t, 2) * StartPoint.Y + 2 * (1 - t) * t * ControlPoint.Y + MathF.Pow(t, 2) * EndPoint.Y;

      return new Vector2(x, y);
    }

    public bool Equals(QuadraticBezierCurve other) =>
        StartPoint.Equals(other.StartPoint) &&
        ControlPoint.Equals(other.ControlPoint) &&
        EndPoint.Equals(other.EndPoint);

    public override bool Equals(object obj) => obj is QuadraticBezierCurve other && Equals(other);
    public override int  GetHashCode()      => HashCode.Combine(StartPoint, ControlPoint, EndPoint);

    public static bool operator ==(QuadraticBezierCurve left, QuadraticBezierCurve right) => left.Equals(right);
    public static bool operator !=(QuadraticBezierCurve left, QuadraticBezierCurve right) => !left.Equals(right);
  }
}