namespace Surreal.Graphics.Textures;

/// <summary>Describes a <see cref="RenderTexture"/>'s underlying texture format.</summary>
public readonly record struct RenderTextureFormat(TextureFormat TextureFormat, TextureFilterMode FilterMode)
{
  public static RenderTextureFormat Default { get; } = new(TextureFormat.Rgba8888, TextureFilterMode.Linear);
}

/// <summary>A texture that can be used for off-screen rendering.</summary>
public sealed class RenderTexture : GraphicsResource
{
  private readonly IGraphicsServer server;
  private readonly GraphicsHandle  handle;

  public RenderTexture(IGraphicsServer server, RenderTextureFormat format)
  {
    this.server = server;
    handle      = server.CreateRenderTexture();

    Format = format;
  }

  public RenderTextureFormat Format { get; }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      server.DeleteRenderTexture(handle);
    }

    base.Dispose(managed);
  }
}
