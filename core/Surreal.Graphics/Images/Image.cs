using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Surreal.Assets;
using Surreal.Graphics.Textures;
using Surreal.IO;
using Surreal.Mathematics;
using Surreal.Memory;
using Size = Surreal.Memory.Size;

namespace Surreal.Graphics.Images;

/// <summary>An image of manipulable pixels that can also be used for a texture.</summary>
[DebuggerDisplay("Image {Width}x{Height} ~{Size}")]
public sealed class Image : ITextureData, IDisposable
{
  private readonly Image<Rgba32> image;

  public static async ValueTask<Image> LoadAsync(VirtualPath path)
  {
    await using var stream = await path.OpenInputStreamAsync();
    var             temp   = await SixLabors.ImageSharp.Image.LoadAsync(stream);

    // we're already in the right format
    if (temp is Image<Rgba32> rgba)
    {
      return new Image(rgba);
    }

    // we need to convert
    using (temp)
    {
      return new Image(temp.CloneAs<Rgba32>());
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

  public int  Width  => image.Width;
  public int  Height => image.Height;
  public Size Size   => Pixels.CalculateSize();

  public Span<Color32> Pixels
  {
    get
    {
      if (!image.TryGetSinglePixelSpan(out var span))
      {
        throw new InvalidOperationException("The image span is not contiguous, unable to access pixels!");
      }

      return MemoryMarshal.Cast<Rgba32, Color32>(span);
    }
  }

  public void Fill(Color32 value)
  {
    Pixels.Fill(value);
  }

  public async ValueTask SaveAsync(VirtualPath path)
  {
    await using var stream = await path.OpenOutputStreamAsync();

    await image.SaveAsPngAsync(stream);
  }

  public void Dispose()
  {
    image.Dispose();
  }

  ReadOnlySpan<Color32> ITextureData.Pixels => Pixels;
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="Image"/>s.</summary>
public sealed class ImageLoader : AssetLoader<Image>
{
  private static ImmutableHashSet<string> Extensions { get; } = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tga" }.ToImmutableHashSet();

  public override bool CanHandle(AssetLoaderContext context)
  {
    return base.CanHandle(context) && Extensions.Contains(context.Path.Extension);
  }

  public override async ValueTask<Image> LoadAsync(AssetLoaderContext context, ProgressToken progressToken = default)
  {
    return await Image.LoadAsync(context.Path);
  }
}
