namespace Surreal.Mathematics;

/// <summary>A curve for utility considerations.</summary>
public delegate float UtilityCurve(float t);

/// <summary>Commonly used <see cref="UtilityCurve"/>s.</summary>
public static class UtilityCurves
{
  public static UtilityCurve Linear(Vector2 a, Vector2 b)
    => _ => throw new NotImplementedException();

  public static UtilityCurve Power(Vector2 a, Vector2 b, float power)
    => _ => throw new NotImplementedException();

  public static UtilityCurve Sigmoid(Vector2 a, Vector2 b, float k)
    => _ => throw new NotImplementedException();
}
