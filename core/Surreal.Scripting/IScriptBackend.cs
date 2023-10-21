namespace Surreal.Scripting;

/// <summary>
/// An abstraction over the different types of scripting backends available.
/// </summary>
public interface IScriptBackend
{
  /// <summary>
  /// A no-op <see cref="IScriptBackend" /> for headless environments and testing.
  /// </summary>
  static IScriptBackend Headless { get; } = new HeadlessScriptBackend();

  /// <summary>
  /// The scripting languages supported by this backend.
  /// </summary>
  IEnumerable<IScriptLanguage> Languages { get; }
}
