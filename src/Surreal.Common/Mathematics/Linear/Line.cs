using System.Numerics;

namespace Surreal.Mathematics.Linear
{
  public readonly record struct Line(Vector2 From, Vector2 To)
  {
    public override string ToString() => $"Line {From.ToString()} to {To.ToString()}";
  }
}
