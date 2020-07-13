using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Surreal.Mathematics.Linear {
  public static class Vectors {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Orthogonal(this Vector2 vector) {
      return new Vector2(-vector.Y, vector.X);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 RotateByDegrees(this Vector2 vector, float degrees) {
      return vector.RotateByRadians(MathF.DegreesToRadians(degrees));
    }

    public static Vector2 RotateByRadians(this Vector2 vector, float radians) {
      var cos = System.MathF.Cos(radians);
      var sin = System.MathF.Sin(radians);

      return new Vector2(cos * vector.X - sin * vector.Y, sin * vector.X + cos * vector.Y);
    }
  }
}