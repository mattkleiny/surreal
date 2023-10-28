using Surreal.Colors;
using Surreal.IO;
using Surreal.Utilities;

namespace Surreal.Editing.Assets.Importers;

[RegisterService(typeof(IAssetImporter))]
internal sealed class ColorPaletteImporter : AssetImporter<ColorPalette>
{
  protected override bool CanHandlePath(string path)
  {
    return path.EndsWith(".pal");
  }

  public override Task<ColorPalette> ImportAsync(VirtualPath path, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
