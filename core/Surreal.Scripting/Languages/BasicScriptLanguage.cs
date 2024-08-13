namespace Surreal.Scripting.Languages;

/// <summary>
/// A <see cref="IScriptLanguage"/> for BASIC.
/// </summary>
public sealed class BasicScriptLanguage : IScriptLanguage
{
  public string Name => "BASIC";

  public ImmutableHashSet<string> SupportedExtensions { get; } = ["bas"];

  public ValueTask<ScriptDeclaration> ParseScriptAsync(TextReader reader, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
