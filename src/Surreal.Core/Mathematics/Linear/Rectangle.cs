﻿using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Surreal.Mathematics.Linear {
  [DebuggerDisplay("Rectangle {Left}, {Top}, {Right}, {Bottom}")]
  public readonly struct Rectangle : IEquatable<Rectangle> {
    public Rectangle(Vector2 min, Vector2 max) {
      Left   = min.X;
      Top    = max.Y;
      Right  = max.X;
      Bottom = min.Y;
    }

    public Rectangle(float left, float top, float right, float bottom) {
      Left   = left;
      Top    = top;
      Right  = right;
      Bottom = bottom;
    }

    public void Deconstruct(out float left, out float top, out float right, out float bottom) {
      left   = Left;
      top    = Top;
      right  = Right;
      bottom = Bottom;
    }

    public float Left   { get; }
    public float Top    { get; }
    public float Right  { get; }
    public float Bottom { get; }

    public float Width  => Right - Left;
    public float Height => Bottom - Top;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(Vector2I point)
      => point.X >= Left &&
         point.X <= Right &&
         point.Y >= Bottom &&
         point.Y <= Top;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(Vector2 vector)
      => vector.X >= Left &&
         vector.X <= Right &&
         vector.Y >= Bottom &&
         vector.Y <= Top;

    public static Rectangle operator *(Rectangle rectangle, float value) => new Rectangle(
      left: rectangle.Left * value,
      top: rectangle.Top * value,
      right: rectangle.Right * value,
      bottom: rectangle.Bottom * value
    );

    public static Rectangle operator *(Rectangle rectangle, Vector2 value) => new Rectangle(
      left: rectangle.Left * value.X,
      top: rectangle.Top * value.Y,
      right: rectangle.Right * value.X,
      bottom: rectangle.Bottom * value.Y
    );

    public bool Equals(Rectangle other) {
      return Left.Equals(other.Left) &&
             Top.Equals(other.Top) &&
             Right.Equals(other.Right) &&
             Bottom.Equals(other.Bottom);
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) return false;

      return obj is Rectangle other && Equals(other);
    }

    public override int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);

    public static bool operator ==(Rectangle left, Rectangle right) => left.Equals(right);
    public static bool operator !=(Rectangle left, Rectangle right) => !left.Equals(right);
  }
}