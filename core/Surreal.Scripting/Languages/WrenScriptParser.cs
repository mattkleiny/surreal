namespace Surreal.Scripting.Languages;

/// <summary>A <see cref="IScriptParser"/> for Wren programs.</summary>
public sealed class WrenScriptParser : IScriptParser
{
  public ValueTask<ScriptDeclaration> ParseScriptAsync(string path, TextReader reader, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
