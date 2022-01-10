using Surreal.Mathematics;

namespace Surreal.UI.Painting;

/// <summary>A brush describes colouring options for painting.</summary>
public record Brush(Color Color)
{
  public static Brush White { get; } = new(Color.White);
  public static Brush Red   { get; } = new(Color.Red);
  public static Brush Green { get; } = new(Color.Green);
  public static Brush Blue  { get; } = new(Color.Blue);
  public static Brush Black { get; } = new(Color.Black);
}
