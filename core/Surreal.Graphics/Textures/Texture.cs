using System;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.IO;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Surreal.Graphics.Textures
{
  public enum TextureFilterMode
  {
    Point,
    Linear,
  }

  public enum TextureFormat
  {
    RGBA8888,
  }

  public enum TextureWrapMode
  {
    Clamp,
    Repeat,
  }

  public interface ITextureData
  {
    TextureFormat Format { get; }

    int  Width  { get; }
    int  Height { get; }
    Size Size   { get; }

    ReadOnlySpan<Color> Pixels { get; }
  }

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

    public void Upload(ITextureData data)
    {
      Upload(this.data, data);

      this.data = data;
    }

    public abstract Image Download();

    protected abstract void Upload(ITextureData? existingData, ITextureData newData);
  }

  public sealed class TextureLoader : AssetLoader<Texture>
  {
    private readonly IGraphicsDevice   device;
    private readonly TextureFilterMode defaultFilterMode;
    private readonly TextureWrapMode   defaultWrapMode;

    public TextureLoader(IGraphicsDevice device, TextureFilterMode defaultFilterMode, TextureWrapMode defaultWrapMode)
    {
      this.device            = device;
      this.defaultFilterMode = defaultFilterMode;
      this.defaultWrapMode   = defaultWrapMode;
    }

    public override async Task<Texture> LoadAsync(Path path, IAssetResolver context)
    {
      var image   = await context.LoadAsset<Image>(path);
      var texture = device.CreateTexture(image.Data, defaultFilterMode, defaultWrapMode);

      return texture;
    }
  }
}