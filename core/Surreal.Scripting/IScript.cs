using Surreal.Assets;

namespace Surreal.Scripting;

/// <summary>Represents an executable script with a convenient API for internal use.</summary>
public interface IScript
{
}

/// <summary>The <see cref="AssetLoader{T}"/> for different <see cref="IScript"/> types.</summary>
public sealed class ScriptLoader : AssetLoader<IScript>
{
  private readonly IScriptCompiler          compiler;
  private readonly ImmutableHashSet<string> extensions;

  public ScriptLoader(IScriptCompiler compiler, params string[] extensions)
    : this(compiler, extensions.AsEnumerable())
  {
  }

  public ScriptLoader(IScriptCompiler compiler, IEnumerable<string> extensions)
  {
    this.compiler   = compiler;
    this.extensions = extensions.ToImmutableHashSet();
  }

  public override bool CanHandle(AssetLoaderContext context)
  {
    return base.CanHandle(context) && extensions.Contains(context.Path.Extension);
  }

  public override async ValueTask<IScript> LoadAsync(AssetLoaderContext context, ProgressToken progressToken = default)
  {
    // TODO: support pre-built binary scripts?

    var declaration = await context.Manager.LoadAssetAsync<ScriptDeclaration>(context.Path);
    var compilation = await compiler.CompileAsync(declaration, progressToken.CancellationToken);

    throw new NotImplementedException();
  }
}
