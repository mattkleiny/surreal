using Surreal.Assets;

namespace Surreal.Graphics.Images;

/// <summary>
/// The <see cref="AssetLoader{T}" /> for <see cref="Image" />s.
/// </summary>
public sealed class ImageLoader : AssetLoader<Image>
{
  private static ImmutableHashSet<string> Extensions { get; } = ImmutableHashSet.Create(".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tga");

  public override bool CanHandle(AssetLoaderContext context)
  {
    return base.CanHandle(context) && Extensions.Contains(context.Path.Extension);
  }

  protected override async Task<Image> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    var image = await Image.LoadAsync(context.Path);

    if (context.IsHotReloadEnabled)
    {
      context.SubscribeToChanges<Image>(ReloadAsync);
    }

    return image;
  }

  private static async Task<Image> ReloadAsync(AssetLoaderContext context, Image image, CancellationToken cancellationToken = default)
  {
    image.ReplaceImage(await Image.LoadAsync(context.Path));

    return image;
  }
}
