using Surreal.Animations;

namespace Surreal.Graphics.Sprites;

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
