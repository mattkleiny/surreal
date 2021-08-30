using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Surreal.Content;
using Surreal.IO;
using Surreal.Mathematics.Linear;

namespace Surreal.Graphics.Textures
{
  /// <summary>Encapsulates a region of a parent <see cref="Texture"/>.</summary>
  public readonly record struct TextureRegion(Texture Texture, Point2 Offset, Point2 Size) : IDisposable
  {
    public int Width  => Size.X;
    public int Height => Size.Y;

    public TextureRegion(Texture texture)
        : this(texture, new(0, 0), new(texture.Width, texture.Height))
    {
    }

    public TextureRegion Slice(Point2 offset, Point2 size)
    {
      return new(Texture, Offset + offset, size);
    }

    public IEnumerable<TextureRegion> Subdivide(int regionWidth, int regionHeight)
    {
      var regionsX = Width / regionWidth;
      var regionsY = Height / regionHeight;

      for (var y = 0; y < regionsY; y++)
      for (var x = 0; x < regionsX; x++)
      {
        yield return new TextureRegion(
            Texture: Texture,
            Offset: new(Offset.X + x * regionWidth, Offset.Y + y * regionHeight),
            Size: new(regionWidth, regionHeight)
        );
      }
    }

    public void Dispose()
    {
      Texture.Dispose();
    }
  }

  /// <summary>The <see cref="AssetLoader{T}"/> for <see cref="TextureRegion"/>s.</summary>
  public sealed class TextureRegionLoader : AssetLoader<TextureRegion>
  {
    public override async Task<TextureRegion> LoadAsync(Path path, IAssetResolver resolver)
    {
      var texture = await resolver.LoadAsset<Texture>(path);

      return texture.Data.ToRegion();
    }
  }
}
