using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Scripting;

/// <summary>The <see cref="AssetLoader{T}"/> for different script types.</summary>
public sealed class ScriptLoader : AssetLoader<ScriptDeclaration>
{
  private readonly IScriptParser            parser;
  private readonly ImmutableHashSet<string> extensions;
  private readonly Encoding                 encoding;

  public ScriptLoader(IScriptParser parser, params string[] extensions)
    : this(parser, extensions.AsEnumerable())
  {
  }

  public ScriptLoader(IScriptParser parser, IEnumerable<string> extensions)
    : this(parser, extensions, Encoding.UTF8)
  {
  }

  public ScriptLoader(IScriptParser parser, IEnumerable<string> extensions, Encoding encoding)
  {
    this.parser     = parser;
    this.extensions = extensions.ToImmutableHashSet();
    this.encoding   = encoding;
  }

  public override bool CanHandle(AssetLoaderContext context)
  {
    return base.CanHandle(context) && extensions.Contains(context.Path.Extension);
  }

  public override async ValueTask<ScriptDeclaration> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken = default)
  {
    // TODO: hot reloading
    await using var stream = await context.Path.OpenInputStreamAsync();

    return await parser.ParseScriptAsync(context.Path.ToString(), stream, encoding, cancellationToken);
  }
}
