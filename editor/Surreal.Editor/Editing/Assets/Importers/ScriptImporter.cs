using Surreal.IO;
using Surreal.Scripting;
using Surreal.Utilities;

namespace Surreal.Editing.Assets.Importers;

[RegisterService(typeof(IAssetImporter))]
internal sealed class ScriptImporter : AssetImporter<Script>
{
  protected override bool CanHandlePath(string path)
  {
    return path.EndsWith(".lua");
  }

  public override Task<Script> ImportAsync(VirtualPath path, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
