using Surreal.IO;

namespace Surreal.Scripting.Languages;

/// <summary>
/// Represents a script language.
/// </summary>
public interface IScriptLanguage
{
  /// <summary>
  /// The name of the language.
  /// </summary>
  string Name { get; }

  /// <summary>
  /// Determines whether the language can load the given path as a script.
  /// </summary>
  bool CanLoad(VirtualPath path);

  /// <summary>
  /// Parses a <see cref="ScriptModule"/> from the given reader.
  /// </summary>
  Task<ScriptModule> ParseAsync(TextReader reader);
}