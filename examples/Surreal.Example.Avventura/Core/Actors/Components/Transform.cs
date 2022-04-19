namespace Avventura.Core.Actors.Components;

/// <summary>A transformed position in 2-space.</summary>
public record struct Transform()
{
  /// <summary>The object's position in 2-space.</summary>
  public Point2 Position = Point2.Zero;
}
