using Surreal.Assets;
using Surreal.Graphics.Images;

namespace Surreal.Graphics.Textures;

/// <summary>
/// Settings for <see cref="Texture" />s.
/// </summary>
public sealed record TextureLoaderSettings
{
  /// <summary>
  /// The format to use for loaded textures.
  /// </summary>
  public TextureFormat Format { get; init; } = TextureFormat.Rgba8;

  /// <summary>
  /// The filter mode to use for loaded textures.
  /// </summary>
  public TextureFilterMode FilterMode { get; init; } = TextureFilterMode.Point;

  /// <summary>
  /// The wrap mode to use for loaded textures.
  /// </summary>
  public TextureWrapMode WrapMode { get; init; } = TextureWrapMode.ClampToEdge;
}

/// <summary>
/// The <see cref="AssetLoader{T}" /> for <see cref="Texture" />s.
/// </summary>
public sealed class TextureLoader(IGraphicsDevice device) : AssetLoader<Texture>
{
  /// <summary>
  /// Default settings for <see cref="Texture"/>s loaded via this loader.
  /// </summary>
  public TextureLoaderSettings Settings { get; init; } = new();

  public override async Task<Texture> LoadAsync(IAssetContext context, CancellationToken cancellationToken)
  {
    var image = await context.LoadAsync<Image>(context.Path, cancellationToken);
    var texture = new Texture(device, Settings.Format, Settings.FilterMode, Settings.WrapMode);

    await texture.WritePixelsAsync(image.Value);

    image.Changed += () =>
    {
      texture.WritePixelsAsync(image.Value);
    };

    return texture;
  }
}
