namespace Isaac.Core.Actors.Components;

/// <summary>A transformed position in 2-space.</summary>
public record struct Transform
{
  private float rotation;

  public Vector2 Position { get; set; }
  public Vector2 Scale    { get; set; }

  public float Rotation
  {
    get => rotation;
    set => rotation = value.Clamp(0, 2 * MathF.PI);
  }
}
