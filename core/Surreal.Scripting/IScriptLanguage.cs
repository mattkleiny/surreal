using Surreal.IO;

namespace Surreal.Scripting;

/// <summary>
/// A language supported that supports scripting.
/// </summary>
public interface IScriptLanguage
{
  /// <summary>
  /// Determines if this language can load the given path.
  /// </summary>
  bool CanLoad(VirtualPath path);

  /// <summary>
  /// Loads a script from the given path.
  /// </summary>
  Task<Script> LoadAsync(VirtualPath path, CancellationToken cancellationToken = default);
}
