namespace Surreal.Scripting.Languages.Lua;

/// <summary>A <see cref="IScriptParser"/> for Lua programs.</summary>
public sealed class LuaScriptParser : IScriptParser
{
  public ValueTask<ScriptDeclaration> ParseScriptAsync(string path, TextReader reader, int length, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
