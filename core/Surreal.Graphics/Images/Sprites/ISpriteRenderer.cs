namespace Surreal.Graphics.Images.Sprites;

/// <summary>A renderer that receives updated sprite details from an animator.</summary>
public interface ISpriteRenderer
{
  /// <summary>The active <see cref="Sprite"/> in this renderer.</summary>
  Sprite? Sprite { get; set; }
}
