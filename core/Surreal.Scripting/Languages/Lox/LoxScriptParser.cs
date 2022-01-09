namespace Surreal.Scripting.Languages.Lox;

/// <summary>A <see cref="IScriptParser"/> for Lox programs.</summary>
public sealed class LoxScriptParser : IScriptParser
{
  public ValueTask<ScriptDeclaration> ParseScriptAsync(string path, TextReader reader, int length, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
