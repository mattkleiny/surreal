using Surreal.Assets;
using Surreal.Graphics.Images;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Surreal.Graphics.Textures;

/// <summary>Filter modes for a <see cref="Texture"/>.</summary>
public enum TextureFilterMode
{
  Point,
  Linear,
}

/// <summary>Formats for a <see cref="Texture"/>.</summary>
public enum TextureFormat
{
  Rgba8888,
}

/// <summary>Wrapping modes for a <see cref="Texture"/>.</summary>
public enum TextureWrapMode
{
  Clamp,
  Repeat,
}

/// <summary>A texture that can be uploaded to the GPU.</summary>
public sealed class Texture : GraphicsResource, IHasSizeEstimate, IDisposableBuffer<Color>, IDisposableBuffer<Color32>
{
  private readonly IGraphicsServer server;

  public Texture(IGraphicsServer server, TextureFormat format, TextureFilterMode filterMode, TextureWrapMode wrapMode)
  {
    this.server = server;

    Format     = format;
    FilterMode = filterMode;
    WrapMode   = wrapMode;

    Handle = server.CreateTexture(filterMode, wrapMode);
  }

  public GraphicsHandle Handle { get; }

  public TextureFormat     Format     { get; }
  public TextureFilterMode FilterMode { get; }
  public TextureWrapMode   WrapMode   { get; }

  public int  Width  { get; private set; }
  public int  Height { get; private set; }
  public Size Size   { get; private set; }

  public TextureRegion ToRegion()
  {
    return new TextureRegion(this);
  }

  public Memory<T> ReadPixels<T>()
    where T : unmanaged
  {
    return server.ReadTextureData<T>(Handle);
  }

  public void WritePixels<T>(int width, int height, ReadOnlySpan<T> pixels)
    where T : unmanaged
  {
    Width  = width;
    Height = height;
    Size   = pixels.CalculateSize();

    server.WriteTextureData(Handle, width, height, pixels, Format);
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

  Memory<Color> IBuffer<Color>.    Memory => ReadPixels<Color>();
  Memory<Color32> IBuffer<Color32>.Memory => ReadPixels<Color32>();
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="Texture"/>s.</summary>
public sealed class TextureLoader : AssetLoader<Texture>
{
  private readonly IGraphicsServer server;
  private readonly TextureFilterMode defaultFilterMode;
  private readonly TextureWrapMode defaultWrapMode;

  public TextureLoader(IGraphicsServer server, TextureFilterMode defaultFilterMode, TextureWrapMode defaultWrapMode)
  {
    this.server            = server;
    this.defaultFilterMode = defaultFilterMode;
    this.defaultWrapMode   = defaultWrapMode;
  }

  public override async ValueTask<Texture> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken = default)
  {
    var image = await context.Manager.LoadAsset<Image>(context.Path, cancellationToken);
    var texture = new Texture(server, TextureFormat.Rgba8888, defaultFilterMode, defaultWrapMode);

    texture.WritePixels(image);

    return texture;
  }
}
