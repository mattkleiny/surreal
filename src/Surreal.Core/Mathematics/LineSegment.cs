using System;
using System.Numerics;

namespace Surreal.Mathematics {
  public readonly struct LineSegment : IEquatable<LineSegment> {
    public Vector2 From { get; }
    public Vector2 To   { get; }

    public LineSegment(Vector2 from, Vector2 to) {
      From = from;
      To   = to;
    }

    public override string ToString() => $"Line {From} to {To}";

    public          bool Equals(LineSegment other) => From.Equals(other.From)  && To.Equals(other.To);
    public override bool Equals(object? obj)       => obj is LineSegment other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(From, To);

    public static bool operator ==(LineSegment left, LineSegment right) => left.Equals(right);
    public static bool operator !=(LineSegment left, LineSegment right) => !left.Equals(right);
  }
}