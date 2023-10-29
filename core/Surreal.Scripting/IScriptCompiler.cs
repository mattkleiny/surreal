namespace Surreal.Scripting;

/// <summary>
/// Abstractly represents a compiled <see cref="IScript"/>.
/// </summary>
public interface ICompiledScript : IScript
{
  /// <summary>
  /// The original path to the script.
  /// </summary>
  string Path { get; }
}

/// <summary>
/// Compiles script code into a different form.
/// </summary>
public interface IScriptCompiler
{
  ValueTask<ICompiledScript> CompileAsync(ScriptDeclaration declaration, CancellationToken cancellationToken = default);
}
