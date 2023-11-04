namespace Surreal.Scripting.Languages;

/// <summary>
/// A <see cref="IScriptLanguage"/> for Lua.
/// </summary>
public sealed class LuaScriptLanguage : IScriptLanguage
{
  public string Name => "Lua";

  public IScriptParser CreateParser()
  {
    return new LuaScriptParser();
  }
}
