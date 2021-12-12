using System.Diagnostics;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Surreal.Assets;
using Surreal.Graphics.Textures;
using Surreal.IO;
using Surreal.Memory;
using Color = Surreal.Mathematics.Color;
using Size = Surreal.Memory.Size;

namespace Surreal.Graphics.Images;

/// <summary>An image of manipulable pixels that can also be used for a texture.</summary>
[DebuggerDisplay("Image {Width}x{Height} ~{Size}")]
public sealed class Image : ITextureData, IDisposable
{
  private readonly Image<Rgba32> image;

  public static async Task<Image> LoadAsync(VirtualPath path)
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
  public Size          Size   => Pixels.CalculateSize();
  public int           Width  => image.Width;
  public int           Height => image.Height;

  public Span<Color> Pixels
  {
    get
    {
      if (!image.TryGetSinglePixelSpan(out var span))
      {
        throw new InvalidOperationException("The image span is not contiguous, unable to access pixels!");
      }

      return MemoryMarshal.Cast<Rgba32, Color>(span);
    }
  }

  public void Fill(Color value)
  {
    Pixels.Fill(value);
  }

  public async Task SaveAsync(VirtualPath path)
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
  public override async Task<Image> LoadAsync(VirtualPath path, IAssetContext context)
  {
    return await Image.LoadAsync(path);
  }
}
