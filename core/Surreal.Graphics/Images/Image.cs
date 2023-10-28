using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Surreal.Assets;
using Surreal.Colors;
using Surreal.IO;
using Surreal.Memory;
using Size = Surreal.Memory.Size;

namespace Surreal.Graphics.Images;

/// <summary>
/// An image of colored pixels.
/// </summary>
[AssetType("834cad39-3394-4ef1-81a7-8cc86335eccd")]
[DebuggerDisplay("Image {Width}x{Height} ~{Size}")]
public sealed class Image : IDisposable
{
  private Image<Rgba32> _image;

  public Image(int width, int height)
  {
    Debug.Assert(width > 0, "width > 0");
    Debug.Assert(height > 0, "height > 0");

    _image = new Image<Rgba32>(width, height);
  }

  private Image(Image<Rgba32> image)
  {
    _image = image;
  }

  /// <summary>
  /// The width of the image, in pixels.
  /// </summary>
  public int Width => _image.Width;

  /// <summary>
  /// The height of the image, in pixels.
  /// </summary>
  public int Height => _image.Height;

  /// <summary>
  /// The size of the image, in bytes.
  /// </summary>
  public Size Size => Pixels.ToSpan().CalculateSize();

  /// <summary>
  /// The underlying pixel data in the image.
  /// </summary>
  public SpanGrid<Color32> Pixels
  {
    get
    {
      if (!_image.TryGetSinglePixelSpan(out var span))
      {
        throw new InvalidOperationException("The image span is not contiguous, unable to access pixels!");
      }

      var pixels = MemoryMarshal.Cast<Rgba32, Color32>(span);

      return new SpanGrid<Color32>(pixels, Width);
    }
  }

  public void Dispose()
  {
    _image.Dispose();
  }

  /// <summary>
  /// Loads an image from the given path.
  /// </summary>
  public static Image Load(VirtualPath path)
  {
    using var stream = path.OpenInputStream();

    // load the image
    var image = SixLabors.ImageSharp.Image.Load(stream);
    if (image is Image<Rgba32> rgba)
    {
      // we're already in the right format
      return new Image(rgba);
    }

    using (image)
    {
      // we need to convert to RGBA
      return new Image(image.CloneAs<Rgba32>());
    }
  }

  /// <summary>
  /// Asynchronously loads an image from the given path.
  /// </summary>
  public static async ValueTask<Image> LoadAsync(VirtualPath path, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenInputStreamAsync();

    // load the image
    var image = await SixLabors.ImageSharp.Image.LoadAsync(Configuration.Default, stream, cancellationToken);
    if (image is Image<Rgba32> rgba)
    {
      // we're already in the right format
      return new Image(rgba);
    }

    // we need to convert to RGBA
    using (image)
    {
      return new Image(image.CloneAs<Rgba32>());
    }
  }

  /// <summary>
  /// Saves the image to the given path.
  /// </summary>
  public void Save(VirtualPath path)
  {
    using var stream = path.OpenOutputStream();

    _image.SaveAsPng(stream);
  }

  /// <summary>
  /// Asynchronously saves the image to the given path.
  /// </summary>
  public async ValueTask SaveAsync(VirtualPath path, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenOutputStreamAsync();

    await _image.SaveAsPngAsync(stream, cancellationToken);
  }

  /// <summary>
  /// Swaps the underlying image content; for hot-reloading.
  /// </summary>
  internal void ReplaceImage(Image other)
  {
    _image = other._image;
  }
}
