using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.IO;
using Surreal.Memory;

namespace Surreal.Graphics.Textures {
  public abstract class Texture : GraphicsResource, IHasSizeEstimate {
    private ITextureData data;

    protected Texture(TextureFormat format, TextureFilterMode filterMode, TextureWrapMode wrapMode) {
      Format     = format;
      FilterMode = filterMode;
      WrapMode   = wrapMode;
    }

    public TextureFormat     Format     { get; }
    public TextureFilterMode FilterMode { get; }
    public TextureWrapMode   WrapMode   { get; }

    public int  Width  => data.Width;
    public int  Height => data.Height;
    public Size Size   => data.Size;

    public TextureRegion ToRegion() => new TextureRegion(this);

    public void Upload(ITextureData data) {
      Upload(this.data, data);

      this.data = data;
    }

    protected abstract void Upload(ITextureData existingData, ITextureData newData);
    public abstract    void Download(Pixmap pixmap);

    public sealed class Loader : AssetLoader<Texture> {
      private readonly IGraphicsDevice   device;
      private readonly TextureFilterMode defaultFilterMode;
      private readonly TextureWrapMode   defaultWrapMode;

      public Loader(
          IGraphicsDevice device,
          TextureFilterMode defaultFilterMode = TextureFilterMode.Point,
          TextureWrapMode defaultWrapMode = TextureWrapMode.Repeat) {
        this.device            = device;
        this.defaultFilterMode = defaultFilterMode;
        this.defaultWrapMode   = defaultWrapMode;
      }

      public override async Task<Texture> LoadAsync(Path path, IAssetLoaderContext context) {
        var pixmap  = await context.GetAsync<Pixmap>(path);
        var texture = device.Factory.CreateTexture(pixmap, defaultFilterMode, defaultWrapMode);

        return texture;
      }
    }
  }
}