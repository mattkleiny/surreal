namespace Surreal.Scripting.Languages;

/// <summary>
/// A <see cref="IScriptLanguage"/> for Lox.
/// </summary>
public sealed class LoxScriptLanguage : IScriptLanguage
{
  public string Name => "Lox";

  public IScriptParser CreateParser()
  {
    return new LoxScriptParser();
  }
}
