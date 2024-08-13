namespace Surreal.Scripting.Languages;

/// <summary>
/// A <see cref="IScriptLanguage"/> for Lisp.
/// </summary>
public sealed class LispScriptLanguage : IScriptLanguage
{
  public string Name => "Lisp";

  public ImmutableHashSet<string> SupportedExtensions { get; } = ["lisp"];

  public ValueTask<ScriptDeclaration> ParseScriptAsync(TextReader reader, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
