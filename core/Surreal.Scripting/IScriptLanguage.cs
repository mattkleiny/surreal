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
  /// The file extensions supported by the language that this parser handles.
  /// </summary>
  ImmutableHashSet<string> SupportedExtensions { get; }

  /// <summary>
  /// Parses a script from the given <see cref="TextReader" />.
  /// </summary>
  ValueTask<ScriptDeclaration> ParseScriptAsync(TextReader reader, CancellationToken cancellationToken = default);
}
