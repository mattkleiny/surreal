using Surreal.Assets;
using Surreal.Graphics.Textures;
using Surreal.IO;

namespace Surreal.Graphics.Fonts;

/// <summary>
/// The <see cref="AssetLoader{T}" /> for <see cref="BitmapFont" />s.
/// </summary>
public sealed class BitmapFontLoader : AssetLoader<BitmapFont>
{
  protected override async Task<BitmapFont> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    var descriptor = await context.Path.DeserializeJsonAsync<BitmapFontDescriptor>(cancellationToken);

    var imagePath = GetImagePath(context.Path, descriptor);
    var texture = await context.LoadAsync<Texture>(imagePath, cancellationToken);

    return new BitmapFont(descriptor, texture);
  }

  private static VirtualPath GetImagePath(VirtualPath descriptorPath, BitmapFontDescriptor descriptor)
  {
    if (descriptor.FilePath != null)
    {
      if (Path.IsPathRooted(descriptor.FilePath))
      {
        return descriptor.FilePath;
      }

      return descriptorPath.GetDirectory().Resolve(descriptor.FilePath);
    }

    return descriptorPath.ChangeExtension("png");
  }
}
