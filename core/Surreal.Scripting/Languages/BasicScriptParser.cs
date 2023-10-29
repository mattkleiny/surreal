namespace Surreal.Scripting.Languages;

/// <summary>
/// A <see cref="IScriptParser"/> for BASIC programs.
/// </summary>
public sealed class BasicScriptParser : IScriptParser
{
  public string[] SupportedExtensions { get; } = { ".bas", ".basic" };

  public ValueTask<ScriptDeclaration> ParseScriptAsync(string path, TextReader reader, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
