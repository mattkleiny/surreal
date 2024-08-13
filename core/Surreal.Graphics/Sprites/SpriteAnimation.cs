using System.Linq.Expressions;
using Surreal.Animations;
using Surreal.Timing;
using Surreal.Utilities;

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

/// <summary>
/// An <see cref="IAnimationTrack"/> for a <see cref="SpriteAnimation"/>.
/// </summary>
public sealed class SpriteAnimationTrack(IProperty<SpriteFrame> property) : KeyFrameAnimationTrack<SpriteFrame>(property)
{
  /// <summary>
  /// The underlying animation.
  /// </summary>
  public required SpriteAnimation Animation { get; init; }

  protected override SpriteFrame UpdateCurrentValue(float currentTime)
  {
    throw new NotImplementedException();
  }
}
