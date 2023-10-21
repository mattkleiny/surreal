using Surreal.Scripting.Lua;

namespace Surreal.Scripting;

/// <summary>
/// A no-op <see cref="IScriptBackend" /> for headless environments and testing.
/// </summary>
internal sealed class HeadlessScriptBackend : IScriptBackend
{
  public IEnumerable<IScriptLanguage> Languages { get; } = new[]
  {
    new LuaScriptLanguage()
  };
}
