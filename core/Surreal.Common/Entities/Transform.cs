namespace Surreal.Entities;

/// <summary>
/// A transform component.
/// </summary>
public record struct Transform() : IComponent<Transform>
{
  /// <summary>
  /// The position of the transform.
  /// </summary>
  public Vector2 Position { get; set; }

  /// <summary>
  /// The rotation of the transform.
  /// </summary>
  public float Rotation { get; set; }

  /// <summary>
  /// The scale of the transform.
  /// </summary>
  public Vector2 Scale { get; set; } = Vector2.One;
}
