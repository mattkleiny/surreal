using Surreal.Resources;

namespace Surreal.Graphics.Images;

/// <summary>
/// The <see cref="AssetLoader{T}" /> for <see cref="Image" />s.
/// </summary>
public sealed class ImageLoader : AssetLoader<Image>
{
  private static ImmutableHashSet<string> Extensions { get; } = ImmutableHashSet.Create(".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tga");

  public override bool CanHandle(AssetContext context)
  {
    return base.CanHandle(context) && Extensions.Contains(context.Path.Extension);
  }

  public override async Task<Image> LoadAsync(AssetContext context, CancellationToken cancellationToken)
  {
    return await Image.LoadAsync(context.Path, cancellationToken);
  }
}
