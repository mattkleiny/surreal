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
  private readonly IScriptParser _parser;
  private readonly IScriptCompiler _compiler;
  private readonly ImmutableHashSet<string> _extensions;

  public ScriptLoader(IScriptParser parser, IScriptCompiler compiler)
    : this(parser, compiler, parser.SupportedExtensions)
  {
  }

  public ScriptLoader(IScriptParser parser, IScriptCompiler compiler, params string[] extensions)
    : this(parser, compiler, extensions.AsEnumerable())
  {
  }

  public ScriptLoader(IScriptParser parser, IScriptCompiler compiler, IEnumerable<string> extensions)
  {
    _parser = parser;
    _compiler = compiler;
    _extensions = extensions.ToImmutableHashSet();
  }

  /// <summary>
  /// The <see cref="IScriptTransformer"/>s to apply to the loaded scripts.
  /// </summary>
  public List<IScriptTransformer> Transformers { get; init; } = new();

  public override bool CanHandle(AssetContext context)
  {
    return base.CanHandle(context) && _extensions.Contains(context.Path.Extension);
  }

  public override async Task<IScript> LoadAsync(AssetContext context, CancellationToken cancellationToken)
  {
    var baseDeclaration = await _parser.ParseScriptAsync(context.Path, cancellationToken);
    var finalDeclaration = await TransformScriptAsync(baseDeclaration, cancellationToken);

    return await _compiler.CompileAsync(finalDeclaration, cancellationToken);
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
