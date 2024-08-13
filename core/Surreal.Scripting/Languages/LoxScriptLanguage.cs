namespace Surreal.Scripting.Languages;

/// <summary>
/// A <see cref="IScriptLanguage"/> for Lox.
/// </summary>
public sealed class LoxScriptLanguage : IScriptLanguage
{
  public string Name => "Lox";

  public ImmutableHashSet<string> SupportedExtensions { get; } = ["lox"];

  public ValueTask<ScriptDeclaration> ParseScriptAsync(TextReader reader, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
