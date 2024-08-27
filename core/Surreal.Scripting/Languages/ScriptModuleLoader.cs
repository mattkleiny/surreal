using Surreal.Assets;
using Surreal.IO;
using Surreal.Scripting.Languages.Lox;

namespace Surreal.Scripting.Languages;

/// <summary>
/// An asset loader for <see cref="ScriptModule"/>s.
/// </summary>
public sealed class ScriptModuleLoader(IEnumerable<IScriptLanguage> languages) : AssetLoader<ScriptModule>
{
  public override bool CanHandle(AssetId id)
  {
    return base.CanHandle(id) && languages.Any(language => language.CanLoad(id.Path));
  }

  public override async Task<ScriptModule> LoadAsync(IAssetContext context, CancellationToken cancellationToken)
  {
    var language = languages.First(language => language.CanLoad(context.Path));

    using var reader = context.Path.OpenInputStreamReader();

    return await language.ParseAsync(reader);
  }
}
