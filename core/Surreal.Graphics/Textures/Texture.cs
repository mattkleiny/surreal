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

/// <summary>A type that supports the data format required for <see cref="Texture"/>s.</summary>
public interface ITextureData
{
  int  Width  { get; }
  int  Height { get; }
  Size Size   { get; }

  ReadOnlySpan<Color32> Pixels { get; }
}

/// <summary>A texture that can be uploaded to the GPU.</summary>
public sealed class Texture : GraphicsResource, IHasSizeEstimate
{
  private readonly GraphicsHandle  handle;
  private readonly IGraphicsServer server;

  public Texture(IGraphicsServer server, TextureFormat format, TextureFilterMode filterMode, TextureWrapMode wrapMode)
  {
    this.server = server;

    handle = server.CreateTexture();

    Format     = format;
    FilterMode = filterMode;
    WrapMode   = wrapMode;
  }

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

  public Image Download()
  {
    throw new NotImplementedException();
  }

  public void Upload(ITextureData data)
  {
    Width  = data.Width;
    Height = data.Height;
    Size   = data.Size;

    server.UploadTextureData(handle, data.Width, data.Height, data.Pixels, Format);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      server.DeleteTexture(handle);
    }

    base.Dispose(managed);
  }
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="Texture"/>s.</summary>
public sealed class TextureLoader : AssetLoader<Texture>
{
  private readonly IGraphicsServer   server;
  private readonly TextureFilterMode defaultFilterMode;
  private readonly TextureWrapMode   defaultWrapMode;

  public TextureLoader(IGraphicsServer server, TextureFilterMode defaultFilterMode, TextureWrapMode defaultWrapMode)
  {
    this.server            = server;
    this.defaultFilterMode = defaultFilterMode;
    this.defaultWrapMode   = defaultWrapMode;
  }

  public override async ValueTask<Texture> LoadAsync(AssetLoaderContext context, ProgressToken progressToken = default)
  {
    // TODO: support hot reloading?
    var image   = await context.Manager.LoadAssetAsync<Image>(context.Path, progressToken);
    var texture = new Texture(server, TextureFormat.Rgba8888, defaultFilterMode, defaultWrapMode);

    texture.Upload(image);

    return texture;
  }
}
