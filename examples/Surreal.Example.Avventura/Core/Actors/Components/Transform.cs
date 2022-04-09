namespace Avventura.Core.Actors.Components;

/// <summary>A transformed position in 2-space.</summary>
public record struct Transform
{
  /// <summary>The object's position in 2-space.</summary>
  public Vector2 Position = Vector2.Zero;

  /// <summary>The object's scale in 2-space.</summary>
  public Vector2 Scale = Vector2.One;

  /// <summary>The rotation about the Y-axis in 2-space.</summary>
  public float Rotation = 0f;
}
