using Surreal.Scripting.VirtualMachine;

namespace Surreal.Scripting;

/// <summary>
/// Compiles script code into a different form.
/// </summary>
public interface IScriptCompiler
{
  /// <summary>
  /// Compiles a script from the given declaration.
  /// </summary>
  ValueTask<BytecodeProgram> CompileAsync(ScriptDeclaration declaration, CancellationToken cancellationToken = default);
}
