namespace Surreal.Scripting;

/// <summary>
/// Compiles script code into a different form.
/// </summary>
public interface IScriptCompiler
{
  ValueTask<ICompiledScript> CompileAsync(ScriptDeclaration declaration, CancellationToken cancellationToken = default);
}

/// <summary>
/// Abstractly represents a compiled <see cref="ScriptDeclaration"/>.
/// </summary>
public interface ICompiledScript
{
  /// <summary>
  /// The original path to the script.
  /// </summary>
  string Path { get; }
}
