using Surreal.Maths;
using Surreal.Memory;
using Color = Surreal.Colors.Color;
using Image = Surreal.Graphics.Images.Image;
using Size = Surreal.Memory.Size;

namespace Surreal.Graphics.Textures;

/// <summary>
/// Formats for a <see cref="Texture" />.
/// </summary>
public enum TextureFormat
{
  R8,
  Rgb8,
  Rgba8,
  R,
  Rgb,
  Rgba
}

/// <summary>
/// Wrapping modes for a <see cref="Texture" />.
/// </summary>
public enum TextureWrapMode
{
  Clamp,
  Repeat
}

/// <summary>
/// Filter modes for a <see cref="Texture" />.
/// </summary>
public enum TextureFilterMode
{
  Point,
  Linear
}

/// <summary>
/// A texture that can be uploaded to the GPU.
/// </summary>
[DebuggerDisplay("Texture {Width}x{Height} (Format {Format})")]
public sealed class Texture(IGraphicsBackend backend, TextureFormat format, TextureFilterMode filterMode, TextureWrapMode wrapMode) : GraphicsAsset, IHasSizeEstimate
{
  public int Width { get; private set; }
  public int Height { get; private set; }
  public GraphicsHandle Handle { get; } = backend.CreateTexture(filterMode, wrapMode);
  public TextureFormat Format { get; set; } = format;

  public TextureFilterMode FilterMode
  {
    get => filterMode;
    set
    {
      filterMode = value;
      backend.SetTextureFilterMode(Handle, value);
    }
  }

  public TextureWrapMode WrapMode
  {
    get => wrapMode;
    set
    {
      wrapMode = value;
      backend.SetTextureWrapMode(Handle, value);
    }
  }

  public Size Size { get; private set; }

  /// <summary>
  /// Creates a colored 1x1 texture.
  /// </summary>
  public static Texture CreateColored(IGraphicsBackend backend, Color color, TextureFormat format = TextureFormat.Rgba8)
  {
    var texture = new Texture(backend, format, TextureFilterMode.Point, TextureWrapMode.Clamp);

    texture.WritePixels<Color>(1, 1, stackalloc Color[] { color });

    return texture;
  }

  /// <summary>
  /// Creates a texture from random noise.
  /// </summary>
  public static Texture CreateNoise(IGraphicsBackend backend, int width, int height, Seed seed = default, TextureFormat format = TextureFormat.Rgba8)
  {
    var texture = new Texture(backend, format, TextureFilterMode.Point, TextureWrapMode.Clamp);
    var random = seed.ToRandom();

    var pixels = new SpanGrid<Color>(new Color[width * height], width);

    for (var y = 0; y < height; y++)
    for (var x = 0; x < width; x++)
    {
      var color = random.NextFloat();

      pixels[x, y] = new Color(color, color, color);
    }

    texture.WritePixels<Color>(width, height, pixels);

    return texture;
  }

  public TextureRegion ToRegion()
  {
    return new TextureRegion(this);
  }

  public Memory<T> ReadPixels<T>()
    where T : unmanaged
  {
    return backend.ReadTextureData<T>(Handle);
  }

  public void ReadPixels<T>(Span<T> buffer)
    where T : unmanaged
  {
    backend.ReadTextureData(Handle, buffer);
  }

  public Memory<T> ReadPixelsSub<T>(int offsetX, int offsetY, int width, int height)
    where T : unmanaged
  {
    return backend.ReadTextureSubData<T>(Handle, offsetX, offsetY, (uint)width, (uint)height);
  }

  public void ReadPixelsSub<T>(Span<T> buffer, int offsetX, int offsetY, int width, int height)
    where T : unmanaged
  {
    backend.ReadTextureSubData(Handle, buffer, offsetX, offsetY, (uint)width, (uint)height);
  }

  public void WritePixels<T>(int width, int height, ReadOnlySpan<T> pixels)
    where T : unmanaged
  {
    Width = width;
    Height = height;
    Size = pixels.CalculateSize();

    backend.WriteTextureData(Handle, (uint)width, (uint)height, pixels, Format);
  }

  public void WritePixelsSub<T>(int offsetX, int offsetY, int width, int height, ReadOnlySpan<T> pixels)
    where T : unmanaged
  {
    backend.WriteTextureSubData(Handle, offsetX, offsetY, (uint)width, (uint)height, pixels, Format);
  }

  public void WritePixels(Image image)
  {
    var pixels = image.Pixels.ToReadOnlySpan();

    WritePixels(image.Width, image.Height, pixels);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      backend.DeleteTexture(Handle);
    }

    base.Dispose(managed);
  }
}
