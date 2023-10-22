using Surreal.Assets;
using Surreal.Utilities;

namespace Surreal.Scripting;

/// <summary>
/// The <see cref="AssetLoader{T}" /> for <see cref="Script" />s.
/// </summary>
[RegisterService(typeof(IAssetLoader))]
public sealed class ScriptLoader(IEnumerable<IScriptLanguage> languages) : AssetLoader<Script>
{
  public override async Task<Script> LoadAsync(AssetContext context, CancellationToken cancellationToken)
  {
    foreach (var language in languages)
    {
      if (language.CanLoad(context.Path))
      {
        return await language.LoadAsync(context.Path, cancellationToken);
      }
    }

    throw new InvalidOperationException("Unable to find a script language for the given path.");
  }
}
