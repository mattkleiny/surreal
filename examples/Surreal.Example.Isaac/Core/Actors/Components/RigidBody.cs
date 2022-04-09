namespace Isaac.Core.Actors.Components;

/// <summary>A transformed rigidbody in 2-space.</summary>
public record struct RigidBody()
{
  /// <summary>True if rigidbody physics are enabled.</summary>
  public bool IsEnabled = true;

  /// <summary>The object's velocity in 2-space.</summary>
  public Vector2 Velocity = Vector2.Zero;

  /// <summary>The object's force in 2-space.</summary>
  public Vector2 Force = Vector2.Zero;

  /// <summary>This object's own gravity.</summary>
  public Vector2? LocalGravity = null;

  /// <summary>The object's mass.</summary>
  public float Mass = 1f;
}
