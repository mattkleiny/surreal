namespace Isaac.Core.Actors.Components;

/// <summary>A sprite to be rendered in 2-space.</summary>
public record struct Sprite()
{
  /// <summary>The underlying glyph for this sprite</summary>
  public Glyph Glyph = 'X';
}
