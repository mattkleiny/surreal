namespace Surreal.Graphics.Sprites;

/// <summary>
/// A single sprite animation.
/// </summary>
public sealed record SpriteAnimation
{
  /// <summary>
  /// The name of the animation.
  /// </summary>
  public required string Name { get; init; }

  /// <summary>
  /// The frames of the animation.
  /// </summary>
  public required ImmutableArray<SpriteFrame> Frames { get; init; } = [];
}

/// <summary>
/// A set of sprite animations.
/// </summary>
public sealed class SpriteAnimationSet
{
  private readonly List<SpriteAnimation> _animations = [];
}
