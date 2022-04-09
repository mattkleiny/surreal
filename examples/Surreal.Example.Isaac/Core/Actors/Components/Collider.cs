namespace Isaac.Core.Actors.Components;

/// <summary>A transformed collider in 2-space.</summary>
public record struct Collider()
{
  /// <summary>True if collision is enabled.</summary>
  public bool IsEnabled = true;

  /// <summary>The collider offset from the center of the object.</summary>
  public Vector2 Offset = Vector2.Zero;

  /// <summary>The collider rectangle size.</summary>
  public Vector2 Size = Vector2.One;
}
