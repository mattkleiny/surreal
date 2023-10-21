using Surreal.IO;

namespace Surreal.Scripting;

/// <summary>
/// A language supported that supports scripting.
/// </summary>
public interface IScriptLanguage
{
  /// <summary>
  /// The name of the language.
  /// </summary>
  string Name { get; }

  /// <summary>
  /// Executes a script in this language.
  /// </summary>
  Variant ExecuteCode(string code);

  /// <summary>
  /// Executes a script file in this language.
  /// </summary>
  Variant ExecuteFile(VirtualPath path);
}
