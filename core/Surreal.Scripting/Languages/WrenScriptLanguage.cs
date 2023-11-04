namespace Surreal.Scripting.Languages;

/// <summary>
/// A <see cref="IScriptLanguage"/> for Wren.
/// </summary>
public sealed class WrenScriptLanguage : IScriptLanguage
{
  public string Name => "Wren";

  public IScriptParser CreateParser()
  {
    return new WrenScriptParser();
  }
}
