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
  /// Determines if this language can load the given path.
  /// </summary>
  bool CanLoad(VirtualPath path);

  /// <summary>
  /// Loads a script from the given path.
  /// </summary>
  Task<Script> LoadAsync(VirtualPath path, CancellationToken cancellationToken = default);

  /// <summary>
  /// Executes a script in this language.
  /// </summary>
  Variant ExecuteCode(string code);

  /// <summary>
  /// Executes a script file in this language.
  /// </summary>
  Variant ExecuteFile(VirtualPath path);
}
