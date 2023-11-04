namespace Surreal.Scripting;

/// <summary>
/// A scripting language.
/// </summary>
public interface IScriptLanguage
{
  /// <summary>
  /// The name of the scripting language.
  /// </summary>
  string Name { get; }

  /// <summary>
  /// Creates a new <see cref="IScriptParser"/> for this language.
  /// </summary>
  IScriptParser CreateParser();
}
