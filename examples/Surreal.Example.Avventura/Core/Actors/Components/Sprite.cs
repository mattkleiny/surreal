namespace Avventura.Core.Actors.Components;

/// <summary>A sprite to be rendered in 2-space.</summary>
public record struct Sprite()
{
  /// <summary>The glyph to apply to the sprite.</summary>
  public Glyph Glyph = new('X');
}
