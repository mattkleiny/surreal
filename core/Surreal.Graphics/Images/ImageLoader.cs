using Surreal.Resources;

namespace Surreal.Graphics.Images;

/// <summary>
/// The <see cref="ResourceLoader{T}" /> for <see cref="Image" />s.
/// </summary>
public sealed class ImageLoader : ResourceLoader<Image>
{
  private static ImmutableHashSet<string> Extensions { get; } = ImmutableHashSet.Create(".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tga");

  public override bool CanHandle(ResourceContext context)
  {
    return base.CanHandle(context) && Extensions.Contains(context.Path.Extension);
  }

  public override async Task<Image> LoadAsync(ResourceContext context, CancellationToken cancellationToken)
  {
    var image = await Image.LoadAsync(context.Path, cancellationToken);

    if (context.IsHotReloadEnabled)
    {
      context.SubscribeToChanges<Image>(ReloadAsync);
    }

    return image;
  }

  private static async Task<Image> ReloadAsync(ResourceContext context, Image image, CancellationToken cancellationToken = default)
  {
    var replacement = await Image.LoadAsync(context.Path, cancellationToken);

    image.ReplaceImage(replacement);

    return image;
  }
}
