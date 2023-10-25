using Surreal.Assets;
using Surreal.Graphics.Images;
using Surreal.Utilities;

namespace Surreal.Graphics.Textures;

/// <summary>
/// Settings for <see cref="Texture" />s.
/// </summary>
public sealed record TextureLoaderSettings
{
  public TextureFormat Format { get; init; } = TextureFormat.Rgba8;
  public TextureFilterMode FilterMode { get; init; } = TextureFilterMode.Point;
  public TextureWrapMode WrapMode { get; init; } = TextureWrapMode.ClampToEdge;
}

/// <summary>
/// The <see cref="AssetLoader{T}" /> for <see cref="Texture" />s.
/// </summary>
[RegisterService(typeof(IAssetLoader))]
public sealed class TextureLoader(IGraphicsBackend graphics) : AssetLoader<Texture>
{
  /// <summary>
  /// Default settings for <see cref="Texture" />s loaded via this loader.
  /// </summary>
  public TextureLoaderSettings Settings { get; init; } = new();

  public override async Task<Texture> LoadAsync(AssetContext context, CancellationToken cancellationToken)
  {
    var image = await context.LoadAsync<Image>(context.Path, cancellationToken);
    var texture = new Texture(graphics, Settings.Format, Settings.FilterMode, Settings.WrapMode);

    texture.WritePixels(image);
    // context.WatchForChanges(context.Path, texture);

    return texture;
  }
}
