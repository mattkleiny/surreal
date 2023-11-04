namespace Surreal.Scripting.Languages;

/// <summary>
/// A <see cref="IScriptLanguage"/> for BASIC.
/// </summary>
public sealed class BasicScriptLanguage : IScriptLanguage
{
  public string Name => "BASIC";

  public IScriptParser CreateParser()
  {
    return new BasicScriptParser();
  }
}
