using Surreal.Assets;
using Surreal.Graphics.Images;
using Surreal.IO;
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
  int           Width  { get; }
  int           Height { get; }
  TextureFormat Format { get; }
  Size          Size   { get; }

  ReadOnlySpan<Color32> Pixels { get; }
}

/// <summary>A texture that can be uploaded to the GPU.</summary>
public abstract class Texture : GraphicsResource, IHasSizeEstimate
{
  private ITextureData? data;

  protected Texture(TextureFormat format, TextureFilterMode filterMode, TextureWrapMode wrapMode)
  {
    Format     = format;
    FilterMode = filterMode;
    WrapMode   = wrapMode;
  }

  public TextureFormat     Format     { get; }
  public TextureFilterMode FilterMode { get; }
  public TextureWrapMode   WrapMode   { get; }

  public int  Width  => data?.Width ?? 0;
  public int  Height => data?.Height ?? 0;
  public Size Size   => data?.Size ?? Size.Zero;

  public TextureRegion ToRegion()
  {
    return new TextureRegion(this);
  }

  public abstract Image Download();

  public void Upload(ITextureData data)
  {
    Upload(this.data, data);

    this.data = data;
  }

  protected abstract void Upload(ITextureData? existingData, ITextureData newData);
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="Texture"/>s.</summary>
public sealed class TextureLoader : AssetLoader<Texture>
{
  private readonly IGraphicsDevice   device;
  private readonly TextureFilterMode defaultFilterMode;
  private readonly TextureWrapMode   defaultWrapMode;
  private readonly bool              hotReloading;

  public TextureLoader(IGraphicsDevice device, TextureFilterMode defaultFilterMode, TextureWrapMode defaultWrapMode, bool hotReloading)
  {
    this.device            = device;
    this.defaultFilterMode = defaultFilterMode;
    this.defaultWrapMode   = defaultWrapMode;
    this.hotReloading      = hotReloading;
  }

  public override async ValueTask<Texture> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken = default)
  {
    var image   = await context.Manager.LoadAssetAsync<Image>(context.Path, cancellationToken);
    var texture = device.CreateTexture(image, defaultFilterMode, defaultWrapMode);

    if (hotReloading && context.Path.GetFileSystem().SupportsWatcher)
    {
      var watcher = context.Path.Watch();

      return new HotLoadingTexture(this, texture, watcher);
    }

    return texture;
  }

  /// <summary>A <see cref="Texture"/> that supports hot reloading from the file system.</summary>
  private sealed class HotLoadingTexture : Texture
  {
    private readonly TextureLoader loader;
    private          Texture?      texture;
    private readonly IPathWatcher  watcher;

    public HotLoadingTexture(TextureLoader loader, Texture texture, IPathWatcher watcher)
      : base(texture.Format, texture.FilterMode, texture.WrapMode)
    {
      this.loader  = loader;
      this.texture = texture;
      this.watcher = watcher;

      watcher.Created  += OnTextureModified;
      watcher.Modified += OnTextureModified;
      watcher.Deleted  += OnTextureDeleted;
    }

    public override Image Download()
    {
      if (texture == null)
      {
        throw new InvalidOperationException("The texture is no longer valid and cannot be accessed");
      }

      return texture.Download();
    }

    protected override void Upload(ITextureData? existingData, ITextureData newData)
    {
      if (texture == null)
      {
        throw new InvalidOperationException("The texture is no longer valid and cannot be accessed");
      }

      texture.Upload(newData);
    }

    private void OnTextureModified(VirtualPath path)
    {
      // TODO: reload the texture
    }

    private void OnTextureDeleted(VirtualPath path)
    {
      // TODO: delete the texture
    }

    protected override void Dispose(bool managed)
    {
      if (managed)
      {
        watcher.Dispose();
        texture?.Dispose();
      }

      base.Dispose(managed);
    }
  }
}
