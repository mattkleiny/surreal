using Surreal.Assets;

namespace Surreal.Graphics.Images;

/// <summary>
/// The <see cref="AssetLoader{T}" /> for <see cref="Image" />s.
/// </summary>
public sealed class ImageLoader : AssetLoader<Image>
{
  private static ImmutableHashSet<string> Extensions { get; } = [".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tga"];

  public override bool CanHandle(AssetId id)
  {
    return base.CanHandle(id) && Extensions.Contains(id.Path.Extension);
  }

  public override async Task<Image> LoadAsync(IAssetContext context, CancellationToken cancellationToken)
  {
    var image = await Image.LoadAsync(context.Path, cancellationToken);

    context.WhenPathChanged(async reloadToken =>
    {
      await image.ReloadAsync(context.Path, reloadToken);
    });

    return image;
  }
}
