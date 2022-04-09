namespace Isaac.Core.Actors.Components;

/// <summary>A transformed position in 2-space.</summary>
public record struct Transform
{
  public Vector2 Position { get; set; }
  public Vector2 Scale    { get; set; }
  public float   Rotation { get; set; }
}
