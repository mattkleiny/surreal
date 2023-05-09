using Surreal.Colors;
using Surreal.Graphics.Images;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Surreal.Graphics.Textures;

/// <summary>
/// Formats for a <see cref="Texture" />.
/// </summary>
public enum TextureFormat
{
  R8,
  Rg8,
  Rgb8,
  Rgba8,
  R,
  Rg,
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
public sealed class Texture : GraphicsResource, IHasSizeEstimate
{
  private TextureFilterMode _filterMode = TextureFilterMode.Point;
  private TextureWrapMode _wrapMode = TextureWrapMode.Clamp;

  public Texture(
    IGraphicsServer server,
    TextureFormat format = TextureFormat.Rgba8,
    TextureFilterMode filterMode = TextureFilterMode.Point,
    TextureWrapMode wrapMode = TextureWrapMode.Clamp)
  {
    Server = server;
    Format = format;
    Handle = server.Backend.CreateTexture(filterMode, wrapMode);
  }

  public IGraphicsServer Server { get; }

  public int Width { get; private set; }
  public int Height { get; private set; }

  public GraphicsHandle Handle { get; }
  public TextureFormat Format { get; set; }

  public TextureFilterMode FilterMode
  {
    get => _filterMode;
    set
    {
      _filterMode = value;
      Server.Backend.SetTextureFilterMode(Handle, value);
    }
  }

  public TextureWrapMode WrapMode
  {
    get => _wrapMode;
    set
    {
      _wrapMode = value;
      Server.Backend.SetTextureWrapMode(Handle, value);
    }
  }

  public Size Size { get; private set; }

  /// <summary>
  /// Creates a colored 1x1 texture.
  /// </summary>
  public static Texture CreateColored(IGraphicsServer server, ColorF color, TextureFormat format = TextureFormat.Rgba8)
  {
    var texture = new Texture(server, format);

    texture.WritePixels<ColorF>(1, 1, stackalloc ColorF[] { color });

    return texture;
  }

  /// <summary>
  /// Creates a texture from random noise.
  /// </summary>
  public static Texture CreateNoise(IGraphicsServer server, int width, int height, Seed seed = default, TextureFormat format = TextureFormat.Rgba8)
  {
    var texture = new Texture(server, format);
    var random = seed.ToRandom();

    var pixels = new SpanGrid<ColorF>(new ColorF[width * height], width);

    for (var y = 0; y < height; y++)
    for (var x = 0; x < width; x++)
    {
      var color = random.NextFloat();

      pixels[x, y] = new ColorF(color, color, color);
    }

    texture.WritePixels<ColorF>(width, height, pixels);

    return texture;
  }

  public TextureRegion ToRegion()
  {
    return new TextureRegion(this);
  }

  public Memory<T> ReadPixels<T>()
    where T : unmanaged
  {
    return Server.Backend.ReadTextureData<T>(Handle);
  }

  public void ReadPixels<T>(Span<T> buffer)
    where T : unmanaged
  {
    Server.Backend.ReadTextureData(Handle, buffer);
  }

  public Memory<T> ReadPixelsSub<T>(int offsetX, int offsetY, int width, int height)
    where T : unmanaged
  {
    return Server.Backend.ReadTextureSubData<T>(Handle, offsetX, offsetY, width, height);
  }

  public void ReadPixelsSub<T>(Span<T> buffer, int offsetX, int offsetY, int width, int height)
    where T : unmanaged
  {
    Server.Backend.ReadTextureSubData(Handle, buffer, offsetX, offsetY, width, height);
  }

  public void WritePixels<T>(int width, int height, ReadOnlySpan<T> pixels)
    where T : unmanaged
  {
    Width = width;
    Height = height;
    Size = pixels.CalculateSize();

    Server.Backend.WriteTextureData(Handle, width, height, pixels, Format);
  }

  public void WritePixelsSub<T>(int offsetX, int offsetY, int width, int height, ReadOnlySpan<T> pixels)
    where T : unmanaged
  {
    Server.Backend.WriteTextureSubData(Handle, offsetX, offsetY, width, height, pixels, Format);
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
      Server.Backend.DeleteTexture(Handle);
    }

    base.Dispose(managed);
  }
}
