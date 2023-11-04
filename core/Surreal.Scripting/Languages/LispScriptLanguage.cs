namespace Surreal.Scripting.Languages;

/// <summary>
/// A <see cref="IScriptLanguage"/> for Lisp.
/// </summary>
public sealed class LispScriptLanguage : IScriptLanguage
{
  public string Name => "Lisp";

  public IScriptParser CreateParser()
  {
    return new LispScriptParser();
  }
}
