using Surreal.Colors;
using Surreal.IO;

namespace Surreal.Editing.Assets.Importers;

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
