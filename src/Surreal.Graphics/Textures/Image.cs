using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Surreal.Assets;
using Surreal.Grids;
using Surreal.IO;
using Color = Surreal.Mathematics.Color;
using Size = Surreal.Memory.Size;

namespace Surreal.Graphics.Textures
{
  /// <summary>An image of manipulable pixels that can also be used for a texture.</summary>
  [DebuggerDisplay("Image {Width}x{Height} ~{Size}")]
  public sealed class Image : ITextureData, IGrid<Color>, IDisposable
  {
    private readonly Image<Rgba32> image;

    public static async Task<Image> LoadAsync(Path path)
    {
      await using var stream = await path.OpenInputStreamAsync();
      var             image  = await SixLabors.ImageSharp.Image.LoadAsync(stream);

      // we're already in the right format
      if (image is Image<Rgba32> rgba)
      {
        return new Image(rgba);
      }

      // we need to convert
      using (image)
      {
        return new Image(image.CloneAs<Rgba32>());
      }
    }

    public Image(int width, int height)
    {
      Debug.Assert(width > 0, "width > 0");
      Debug.Assert(height > 0, "height > 0");

      image = new Image<Rgba32>(width, height);
    }

    private Image(Image<Rgba32> image)
    {
      this.image = image;
    }

    public TextureFormat Format => TextureFormat.RGBA8888;
    public Span<Color>   Pixels => throw new NotImplementedException();
    public Size          Size   => throw new NotImplementedException();

    public int Width  => image.Width;
    public int Height => image.Height;

    public Color this[int x, int y]
    {
      get
      {
        Debug.Assert(x >= 0 && x < Width, "x >= 0 && x < Width");
        Debug.Assert(y >= 0 && y < Height, "y >= 0 && y < Height");

        return Pixels[x + y * Width];
      }
      set
      {
        Debug.Assert(x >= 0 && x < Width, "x >= 0 && x < Width");
        Debug.Assert(y >= 0 && y < Height, "y >= 0 && y < Height");

        Pixels[x + y * Width] = value;
      }
    }

    public void Fill(Color value)
    {
      Pixels.Fill(value);
    }

    public async Task SaveAsync(Path path)
    {
      await using var stream = await path.OpenOutputStreamAsync();

      await image.SaveAsPngAsync(stream);
    }

    public void Dispose()
    {
      image.Dispose();
    }

    ReadOnlySpan<Color> ITextureData.Pixels => Pixels;
  }

  /// <summary>The <see cref="AssetLoader{T}"/> for <see cref="Image"/>s.</summary>
  public sealed class ImageLoader : AssetLoader<Image>
  {
    public override async Task<Image> LoadAsync(Path path, IAssetResolver context)
    {
      return await Image.LoadAsync(path);
    }
  }
}
