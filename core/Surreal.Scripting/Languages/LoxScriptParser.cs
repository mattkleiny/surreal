namespace Surreal.Scripting.Languages;

/// <summary>
/// A <see cref="IScriptParser"/> for Lox programs.
/// </summary>
public sealed class LoxScriptParser : IScriptParser
{
  public string[] SupportedExtensions { get; } = { ".lox" };

  public ValueTask<ScriptDeclaration> ParseScriptAsync(string path, TextReader reader, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
