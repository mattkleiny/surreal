using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Sprites;

/// <summary>
/// A single frame of a sprite animation.
/// </summary>
public readonly record struct SpriteFrame
{
  /// <summary>
  /// The <see cref="TextureRegion"/> of this frame.
  /// </summary>
  public TextureRegion Texture { get; init; }

  /// <summary>
  /// The duration of this frame.
  /// </summary>
  public TimeSpan Duration { get; init; }
}
