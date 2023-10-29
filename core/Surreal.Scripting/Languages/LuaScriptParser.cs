namespace Surreal.Scripting.Languages;

/// <summary>
/// A <see cref="IScriptParser"/> for Lua programs.
/// </summary>
public sealed class LuaScriptParser : IScriptParser
{
  public ValueTask<ScriptDeclaration> ParseScriptAsync(string path, TextReader reader, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
