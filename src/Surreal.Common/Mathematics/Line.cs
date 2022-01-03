using System.Numerics;

namespace Surreal.Mathematics;

/// <summary>A line in 2-space.</summary>
public readonly record struct Line(Vector2 From, Vector2 To)
{
  public override string ToString() => $"Line {From} to {To}";
}
