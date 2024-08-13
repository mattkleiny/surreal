using Surreal.Assets;

namespace Surreal.Scripting;

/// <summary>
/// Represents an executable script with a convenient API for internal use.
/// </summary>
public interface IScript;

/// <summary>
/// The <see cref="AssetLoader{T}"/> for different <see cref="IScript"/> types.
/// </summary>
public sealed class ScriptLoader(IScriptParser parser, IScriptCompiler compiler, IEnumerable<string> extensions) : AssetLoader<IScript>
{
  private readonly ImmutableHashSet<string> _extensions = extensions.ToImmutableHashSet();

  public ScriptLoader(IScriptParser parser, IScriptCompiler compiler)
    : this(parser, compiler, parser.SupportedExtensions)
  {
  }

  public ScriptLoader(IScriptParser parser, IScriptCompiler compiler, params string[] extensions)
    : this(parser, compiler, extensions.AsEnumerable())
  {
  }

  /// <summary>
  /// The <see cref="IScriptTransformer"/>s to apply to the loaded scripts.
  /// </summary>
  public List<IScriptTransformer> Transformers { get; init; } = [];

  public override bool CanHandle(AssetId id)
  {
    return base.CanHandle(id) && _extensions.Contains(id.Path.Extension);
  }

  public override async Task<IScript> LoadAsync(IAssetContext context, CancellationToken cancellationToken)
  {
    var baseDeclaration = await parser.ParseScriptAsync(context.Path, cancellationToken);
    var finalDeclaration = await TransformScriptAsync(baseDeclaration, cancellationToken);

    return await compiler.CompileAsync(finalDeclaration, cancellationToken);
  }

  private async Task<ScriptDeclaration> TransformScriptAsync(ScriptDeclaration declaration, CancellationToken cancellationToken = default)
  {
    foreach (var transformer in Transformers)
    {
      if (transformer.CanTransform(declaration))
      {
        declaration = await transformer.TransformAsync(declaration, cancellationToken);
      }
    }

    return declaration;
  }
}
