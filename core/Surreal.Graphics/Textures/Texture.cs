using Surreal.Assets;
using Surreal.Graphics.Images;
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
public sealed class Texture : GraphicsResource, IHasSizeEstimate
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
}

/// <summary>Settings for <see cref="Texture"/>s.</summary>
public sealed record TextureSettings : AssetSettings<Texture>
{
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

  public override async ValueTask<Texture> LoadAsync(AssetLoaderContext context, TextureSettings settings, CancellationToken cancellationToken)
  {
    var image = await context.LoadDependencyAsync<Image>(context.Path, cancellationToken);
    var texture = new Texture(server, TextureFormat.Rgba8888, settings.FilterMode, settings.WrapMode);

    texture.WritePixels(image);

    if (context.IsHotReloadEnabled)
    {
      context.SubscribeToChanges<Texture>(ReloadAsync);
    }

    return texture;
  }

  private static async ValueTask<Texture> ReloadAsync(AssetLoaderContext context, Texture texture, CancellationToken cancellationToken = default)
  {
    var image = await context.LoadDependencyAsync<Image>(context.Path, cancellationToken);

    texture.WritePixels(image);

    return texture;
  }
}
