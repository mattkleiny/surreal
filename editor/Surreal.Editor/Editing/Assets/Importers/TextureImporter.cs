using Surreal.Graphics.Textures;
using Surreal.IO;
using Surreal.Utilities;

namespace Surreal.Editing.Assets.Importers;

[RegisterService(typeof(IAssetImporter))]
internal sealed class TextureImporter : AssetImporter<Texture>
{
  protected override bool CanHandlePath(string path)
  {
    return path.EndsWith(".png");
  }

  public override Task<Texture> ImportAsync(VirtualPath path, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
