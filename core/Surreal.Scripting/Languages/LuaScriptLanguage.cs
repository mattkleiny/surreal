namespace Surreal.Scripting.Languages;

/// <summary>
/// A <see cref="IScriptLanguage"/> for Lua.
/// </summary>
public sealed class LuaScriptLanguage : IScriptLanguage
{
  public string Name => "Lua";

  public ImmutableHashSet<string> SupportedExtensions { get; } = ["lua"];

  public ValueTask<ScriptDeclaration> ParseScriptAsync(TextReader reader, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
