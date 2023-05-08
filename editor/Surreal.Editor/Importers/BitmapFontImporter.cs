using Surreal.Assets;
using Surreal.Graphics.Fonts;

namespace Surreal.Editor.Importers;

/// <summary>
/// Imports <see cref="BitmapFont"/> data.
/// </summary>
internal sealed class BitmapFontImporter : AssetImporter<BitmapFont>
{
  public override bool CanHandle(string path)
  {
    return path.EndsWith(".font");
  }

  public override Task<BitmapFont> ImportAsync(string path, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
