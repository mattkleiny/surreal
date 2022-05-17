using System.Buffers;
using Surreal.Assets;
using Surreal.Graphics.Images;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Surreal.Graphics.Textures;

/// <summary>Formats for a <see cref="Texture"/>.</summary>
public enum TextureFormat
{
  R8,
  Rg8,
  Rgb8,
  Rgba8,
  R,
  Rg,
  Rgb,
  Rgba,
}

/// <summary>Wrapping modes for a <see cref="Texture"/>.</summary>
public enum TextureWrapMode
{
  Clamp,
  Repeat
}

/// <summary>Filter modes for a <see cref="Texture"/>.</summary>
public enum TextureFilterMode
{
  Point,
  Linear
}

/// <summary>A texture that can be uploaded to the GPU.</summary>
[DebuggerDisplay("Texture {Width}x{Height} (Format {Format})")]
public sealed class Texture : GraphicsResource, IHasSizeEstimate
{
  /// <summary>Creates a colored 1x1 texture.</summary>
  public static Texture CreateColored(IGraphicsServer server, Color color, TextureFormat format = TextureFormat.Rgba8)
  {
    var texture = new Texture(server, format);

    texture.WritePixels<Color>(1, 1, stackalloc Color[] { color });

    return texture;
  }

  /// <summary>Creates a texture from random noise.</summary>
  public static Texture CreateNoise(IGraphicsServer server, int width, int height, Seed seed = default, TextureFormat format = TextureFormat.Rgba8)
  {
    var texture = new Texture(server, format);
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

  private readonly IGraphicsServer server;
  private TextureFilterMode filterMode = TextureFilterMode.Point;
  private TextureWrapMode wrapMode = TextureWrapMode.Clamp;

  public Texture(
    IGraphicsServer server,
    TextureFormat format = TextureFormat.Rgba8,
    TextureFilterMode filterMode = TextureFilterMode.Point,
    TextureWrapMode wrapMode = TextureWrapMode.Clamp)
  {
    this.server = server;

    Format = format;
    Handle = server.CreateTexture(filterMode, wrapMode);
  }

  public IGraphicsServer Server => server;

  public int  Width  { get; private set; }
  public int  Height { get; private set; }
  public Size Size   { get; private set; }

  public GraphicsHandle Handle { get; }
  public TextureFormat  Format { get; set; }

  public TextureFilterMode FilterMode
  {
    get => filterMode;
    set
    {
      filterMode = value;
      server.SetTextureFilterMode(Handle, value);
    }
  }

  public TextureWrapMode WrapMode
  {
    get => wrapMode;
    set
    {
      wrapMode = value;
      server.SetTextureWrapMode(Handle, value);
    }
  }

  public TextureRegion ToRegion()
  {
    return new TextureRegion(this);
  }

  public TextureDataLease<T> RentPixels<T>()
    where T : unmanaged
  {
    return new TextureDataLease<T>(this);
  }

  public Memory<T> ReadPixels<T>()
    where T : unmanaged
  {
    return server.ReadTextureData<T>(Handle);
  }

  public void ReadPixels<T>(Span<T> buffer)
    where T : unmanaged
  {
    server.ReadTextureData(Handle, buffer);
  }

  public Memory<T> ReadPixelsSub<T>(int offsetX, int offsetY, int width, int height)
    where T : unmanaged
  {
    return server.ReadTextureSubData<T>(Handle, offsetX, offsetY, width, height);
  }

  public void ReadPixelsSub<T>(Span<T> buffer, int offsetX, int offsetY, int width, int height)
    where T : unmanaged
  {
    server.ReadTextureSubData(Handle, buffer, offsetX, offsetY, width, height);
  }

  public void WritePixels<T>(int width, int height, ReadOnlySpan<T> pixels)
    where T : unmanaged
  {
    Width  = width;
    Height = height;
    Size   = pixels.CalculateSize();

    server.WriteTextureData(Handle, width, height, pixels, Format);
  }

  public void WritePixelsSub<T>(int offsetX, int offsetY, int width, int height, ReadOnlySpan<T> pixels)
    where T : unmanaged
  {
    server.WriteTextureSubData(Handle, offsetX, offsetY, width, height, pixels, Format);
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
      server.DeleteTexture(Handle);
    }

    base.Dispose(managed);
  }

  /// <summary>Allows borrowing the texture's data.</summary>
  public readonly struct TextureDataLease<T> : IMemoryOwner<T>
    where T : unmanaged
  {
    private readonly Texture texture;

    public TextureDataLease(Texture texture)
    {
      this.texture = texture;
      Memory       = texture.ReadPixels<T>();
    }

    public Memory<T>   Memory { get; }
    public SpanGrid<T> Span   => Memory.Span.ToGrid(texture.Width);

    public void Dispose()
    {
      texture.WritePixels<T>(texture.Width, texture.Height, Memory.Span);
    }
  }
}

/// <summary>Settings for <see cref="Texture"/>s.</summary>
public sealed record TextureSettings : AssetSettings<Texture>
{
  public TextureFormat     Format     { get; init; } = TextureFormat.Rgba8;
  public TextureFilterMode FilterMode { get; init; } = TextureFilterMode.Point;
  public TextureWrapMode   WrapMode   { get; init; } = TextureWrapMode.Clamp;
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="Texture"/>s.</summary>
public sealed class TextureLoader : AssetLoader<Texture, TextureSettings>
{
  private readonly IGraphicsServer server;

  public TextureLoader(IGraphicsServer server)
  {
    this.server = server;
  }

  public override async Task<Texture> LoadAsync(AssetLoaderContext context, TextureSettings settings, CancellationToken cancellationToken)
  {
    var image = await context.LoadAsync<Image>(context.Path, cancellationToken);
    var texture = new Texture(server, settings.Format, settings.FilterMode, settings.WrapMode);

    texture.WritePixels(image);

    if (context.IsHotReloadEnabled)
    {
      context.SubscribeToChanges<Texture>(ReloadAsync);
    }

    return texture;
  }

  private static async Task<Texture> ReloadAsync(AssetLoaderContext context, Texture texture, CancellationToken cancellationToken = default)
  {
    var image = await context.LoadAsync<Image>(context.Path, cancellationToken);

    texture.WritePixels(image);

    return texture;
  }
}
