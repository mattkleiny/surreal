using Surreal.Colors;

namespace Surreal.Mathematics;

/// <summary>
/// Represents a value that can be interpolated between two values.
/// </summary>
public interface IFromLerp<TSelf>
  where TSelf : IFromLerp<TSelf>
{
  /// <summary>
  /// Interpolates between two values.
  /// </summary>
  static abstract TSelf Lerp(TSelf a, TSelf b, float t);
}

/// <summary>
/// Interpolates between two values.
/// </summary>
public delegate T Interpolator<T>(T from, T to, float amount);

/// <summary>
/// Helpers for performing interpolation.
/// </summary>
public static class Interpolation
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float LerpFloat(float from, float to, float amount)
  {
    return from + (to - from) * amount;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Vector2 LerpVector2(Vector2 from, Vector2 to, float amount)
  {
    var x = LerpFloat(from.X, to.X, amount);
    var y = LerpFloat(from.Y, to.Y, amount);

    return new Vector2(x, y);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Vector3 LerpVector3(Vector3 from, Vector3 to, float amount)
  {
    var x = LerpFloat(from.X, to.X, amount);
    var y = LerpFloat(from.Y, to.Y, amount);
    var z = LerpFloat(from.Z, to.Z, amount);

    return new Vector3(x, y, z);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Vector4 LerpVector4(Vector4 from, Vector4 to, float amount)
  {
    var x = LerpFloat(from.X, to.X, amount);
    var y = LerpFloat(from.Y, to.Y, amount);
    var z = LerpFloat(from.Z, to.Z, amount);
    var w = LerpFloat(from.W, to.W, amount);

    return new Vector4(x, y, z, w);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Color LerpColor(Color from, Color to, float amount)
  {
    var r = LerpFloat(from.R, to.R, amount);
    var g = LerpFloat(from.G, to.G, amount);
    var b = LerpFloat(from.B, to.B, amount);
    var a = LerpFloat(from.A, to.A, amount);

    return new Color(r, g, b, a);
  }
}
