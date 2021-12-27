using System.Numerics;

namespace Surreal.Thinking.Utility;

/// <summary>A curve for utility considerations.</summary>
public delegate float UtilityCurve(float t);

/// <summary>Commonly used <see cref="UtilityCurve"/>s.</summary>
public static class UtilityCurves
{
	public static UtilityCurve Linear(Vector2 pointA, Vector2 pointB) => _ => throw new NotImplementedException();
	public static UtilityCurve Power(Vector2 pointA, Vector2 pointB, float power) => _ => throw new NotImplementedException();
	public static UtilityCurve Sigmoid(Vector2 pointA, Vector2 pointB, float k) => _ => throw new NotImplementedException();
}
