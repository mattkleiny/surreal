using Surreal.Graphics.Textures;

namespace Surreal.Modules.Sprites {
  public sealed class Sprite {
    public Texture ColorMap  { get; }
    public Texture NormalMap { get; }

    public Sprite(Texture colorMap, Texture normalMap) {
      ColorMap  = colorMap;
      NormalMap = normalMap;
    }
  }
}