using System;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Graphics.Textures;
using Surreal.IO;
using Surreal.Mathematics;

namespace Surreal.Graphics.Sprites
{
  public sealed record Sprite(Texture Diffuse, Texture? Normal = default, Pivot Pivot = default);

  public sealed class SpriteLoader : AssetLoader<Sprite>
  {
    public override Task<Sprite> LoadAsync(Path path, IAssetResolver context)
    {
      throw new NotImplementedException();
    }
  }
}