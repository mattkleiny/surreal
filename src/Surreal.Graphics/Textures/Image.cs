using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using Surreal.Assets;
using Surreal.Grids;
using Surreal.IO;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Surreal.Graphics.Textures
{
  [DebuggerDisplay("Image {Width}x{Height} ~{Size}")]
  public sealed class Image : ITextureData, IGrid<Color>, IDisposable
  {
    private readonly IDisposableBuffer<Color> buffer;

    public static async Task<Image> LoadAsync(Path path)
    {
      await using var stream = await path.OpenInputStreamAsync();
      using var       image  = SixLabors.ImageSharp.Image.Load(stream);

      var result = new Image(image.Width, image.Height);

      image.GetPixelSpan().Cast<Rgba32, Color>().CopyTo(result.Pixels);

      return result;
    }

    public Image(int width, int height)
    {
      Debug.Assert(width > 0, "width > 0");
      Debug.Assert(height > 0, "height > 0");

      Width  = width;
      Height = height;

      buffer = Buffers.AllocateNative<Color>(width * height);
    }

    public Size          Size   => buffer.Size;
    public TextureFormat Format => TextureFormat.RGBA8888;
    public Span<Color>   Pixels => buffer.Data;

    public int Width  { get; }
    public int Height { get; }

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
      using var       image  = new Image<Rgba32>(Width, Height);

      Pixels.Cast<Color, Rgba32>().CopyTo(image.GetPixelSpan());

      image.SaveAsPng(stream);
    }

    public void Dispose()
    {
      buffer.Dispose();
    }

    ReadOnlySpan<Color> ITextureData.Pixels => Pixels;
  }

  public sealed class ImageLoader : AssetLoader<Image>
  {
    public override async Task<Image> LoadAsync(Path path, IAssetResolver context)
    {
      return await Image.LoadAsync(path);
    }
  }
}