using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Sprites {
  public sealed class SpriteFrame {
    public TextureRegion Texture { get; }
    public Color         Tint    { get; set; } = Color.White;

    public SpriteFrame(TextureRegion texture) {
      Texture = texture;
    }
  }
}