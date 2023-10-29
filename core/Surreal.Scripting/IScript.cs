using Surreal.Assets;

namespace Surreal.Scripting;

/// <summary>
/// Represents an executable script with a convenient API for internal use.
/// </summary>
public interface IScript
{
}

/// <summary>
/// The <see cref="AssetLoader{T}"/> for different <see cref="IScript"/> types.
/// </summary>
public sealed class ScriptLoader : AssetLoader<IScript>
{
  private readonly IScriptCompiler _compiler;
  private readonly ImmutableHashSet<string> _extensions;

  public ScriptLoader(IScriptCompiler compiler, params string[] extensions)
    : this(compiler, extensions.AsEnumerable())
  {
  }

  public ScriptLoader(IScriptCompiler compiler, IEnumerable<string> extensions)
  {
    _compiler = compiler;
    _extensions = extensions.ToImmutableHashSet();
  }

  public override bool CanHandle(AssetContext context)
  {
    return base.CanHandle(context) && _extensions.Contains(context.Path.Extension);
  }

  public override async Task<IScript> LoadAsync(AssetContext context, CancellationToken cancellationToken)
  {
    // TODO: support pre-built binary scripts?

    var declaration = await context.Manager.LoadAssetAsync<ScriptDeclaration>(context.Path, cancellationToken);
    var compilation = await _compiler.CompileAsync(declaration, cancellationToken);

    throw new NotImplementedException();
  }
}
