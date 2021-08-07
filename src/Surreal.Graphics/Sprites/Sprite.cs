using System;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Graphics.Textures;
using Surreal.IO;
using Surreal.Mathematics;

namespace Surreal.Graphics.Sprites
{
  /// <summary>Represents a sprite.</summary>
  public sealed record Sprite(
      Texture Diffuse,
      Texture? Normal = default,
      Pivot Pivot = default
  );

  /// <summary>The <see cref="AssetLoader{T}"/> for <see cref="Sprite"/>s.</summary>
  public sealed class SpriteLoader : AssetLoader<Sprite>
  {
    public override Task<Sprite> LoadAsync(Path path, IAssetResolver context)
    {
      throw new NotImplementedException();
    }
  }
}
