using Surreal.Assets;
using Surreal.Graphics.Images;

namespace Surreal.Editor.Importers;

/// <summary>
/// Imports <see cref="Image"/> data.
/// </summary>
internal sealed class ImageImporter : AssetImporter<Image>
{
  public override bool CanHandle(string path)
  {
    return path.EndsWith(".png");
  }

  public override Task<Image> ImportAsync(string path, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
