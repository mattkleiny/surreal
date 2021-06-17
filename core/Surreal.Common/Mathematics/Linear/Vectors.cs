using System.Numerics;

namespace Surreal.Mathematics.Linear {
  public static class Vectors {
    public static Vector2 Orthogonal(this Vector2 vector) {
      return new(-vector.Y, vector.X);
    }
  }
}