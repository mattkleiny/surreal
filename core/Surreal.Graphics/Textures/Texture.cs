﻿using Surreal.Assets;
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

  ReadOnlySpan<Color> Pixels { get; }
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

  public override async Task<Texture> LoadAsync(VirtualPath path, IAssetContext context, CancellationToken cancellationToken = default)
  {
    if (hotReloading)
    {
      // TODO: implement hot reloading with a file watcher
    }

    var image   = await context.LoadAsset<Image>(path, cancellationToken);
    var texture = device.CreateTexture(image, defaultFilterMode, defaultWrapMode);

    return texture;
  }
}