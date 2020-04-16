using Surreal.Graphics;
using Surreal.Graphics.Textures;

namespace Surreal.Framework.Scenes.Entities.Components
{
  public struct Sprite : IComponent
  {
    public float          Scale;
    public Color          Tint;
    public TextureRegion? Texture;
    public int            Layer;
  }
}