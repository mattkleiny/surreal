using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Surreal.Mathematics.Linear {
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public struct Matrix2x2 {
    public static readonly Matrix2x2 Identity = new Matrix2x2(
        1, 0,
        0, 1
    );

    public float M1;
    public float M2;
    public float M3;
    public float M4;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix2x2 CreateFromAngles(float sine, float cosine) {
      return new Matrix2x2(cosine, -sine, sine, cosine);
    }

    public Matrix2x2(float m1, float m2, float m3, float m4) {
      M1 = m1;
      M2 = m2;
      M3 = m3;
      M4 = m4;
    }

    public static Vector2 operator *(Matrix2x2 matrix, Vector2 vector) {
      return new Vector2(
          vector.X * matrix.M1 + vector.Y * matrix.M2,
          vector.X * matrix.M3 + vector.Y * matrix.M4
      );
    }
  }
}