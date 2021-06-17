using System;
using System.Numerics;

namespace Surreal.Mathematics.Linear {
  public readonly struct Line : IEquatable<Line> {
    public readonly Vector2 From;
    public readonly Vector2 To;

    public Line(Vector2 from, Vector2 to) {
      From = from;
      To   = to;
    }

    public void Deconstruct(out Vector2 @from, out Vector2 to) {
      from = From;
      to   = To;
    }

    public override string ToString() => $"Line {From} to {To}";

    public          bool Equals(Line other)  => From.Equals(other.From) && To.Equals(other.To);
    public override bool Equals(object? obj) => obj is Line other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(From, To);

    public static bool operator ==(Line left, Line right) => left.Equals(right);
    public static bool operator !=(Line left, Line right) => !left.Equals(right);
  }
}