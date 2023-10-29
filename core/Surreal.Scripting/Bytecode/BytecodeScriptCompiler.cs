namespace Surreal.Scripting.Bytecode;

/// <summary>
/// A <see cref="IScriptCompiler"/> that lowers script code to byte code.
/// </summary>
public sealed class BytecodeScriptCompiler : IScriptCompiler
{
  private readonly IBytecodeScriptOptimizer _optimizer;

  public BytecodeScriptCompiler()
    : this(NullBytecodeScriptOptimizer.Instance)
  {
  }

  public BytecodeScriptCompiler(IBytecodeScriptOptimizer optimizer)
  {
    _optimizer = optimizer;
  }

  public ValueTask<ICompiledScript> CompileAsync(ScriptDeclaration declaration, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  /// <summary>
  /// A no-op <see cref="IBytecodeScriptOptimizer"/> implementation.
  /// </summary>
  private sealed class NullBytecodeScriptOptimizer : IBytecodeScriptOptimizer
  {
    public static NullBytecodeScriptOptimizer Instance { get; } = new();
  }
}
