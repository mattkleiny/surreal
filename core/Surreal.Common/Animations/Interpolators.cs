using System.Runtime.CompilerServices;
using Surreal.Mathematics;

namespace Surreal.Animations;

/// <summary>Interpolates between two values.</summary>
public delegate T Interpolator<T>(T from, T to, float amount);

/// <summary>Commonly used <see cref="Interpolator{T}"/> functions.</summary>
public static class Interpolators
{
  public static Interpolator<int>   IntLinear   { get; } = Maths.Lerp;
  public static Interpolator<float> FloatLinear { get; } = Maths.Lerp;
  public static Interpolator<Color> ColorLinear { get; } = Color.Lerp;

  /// <summary>Selects the default <see cref="Interpolator{T}"/> to use for the given type, <see cref="T"/>.</summary>
  public static Interpolator<T> Default<T>()
  {
    if (typeof(T) == typeof(int))
      return Unsafe.As<Interpolator<T>>(IntLinear);

    if (typeof(T) == typeof(float))
      return Unsafe.As<Interpolator<T>>(FloatLinear);

    if (typeof(T) == typeof(Color))
      return Unsafe.As<Interpolator<T>>(ColorLinear);

    return (a, _, _) => a; // constant
  }
}
