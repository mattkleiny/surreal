using Surreal.Assets;
using Surreal.Graphics.Images;

namespace Surreal.Graphics.Textures;

/// <summary>
/// Settings for <see cref="Texture" />s.
/// </summary>
public sealed record TextureLoaderSettings
{
  public TextureFormat Format { get; init; } = TextureFormat.Rgba8;
  public TextureFilterMode FilterMode { get; init; } = TextureFilterMode.Point;
  public TextureWrapMode WrapMode { get; init; } = TextureWrapMode.Clamp;
}

/// <summary>
/// The <see cref="AssetLoader{T}" /> for <see cref="Texture" />s.
/// </summary>
public sealed class TextureLoader : AssetLoader<Texture>
{
  private readonly IGraphicsServer _server;

  public TextureLoader(IGraphicsServer server)
  {
    _server = server;
  }

  public TextureLoaderSettings Settings { get; init; } = new();

  public override async Task<Texture> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    var image = await context.LoadAsync<Image>(context.Path, cancellationToken);
    var texture = new Texture(_server, Settings.Format, Settings.FilterMode, Settings.WrapMode);

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
