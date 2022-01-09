namespace Surreal.Scripting.Bytecode;

/// <summary>A <see cref="IScriptCompiler"/> that lowers script code to byte code.</summary>
public sealed class BytecodeCompiler : IScriptCompiler
{
  private readonly IBytecodeOptimizer optimizer;

  public BytecodeCompiler()
    : this(NullBytecodeOptimizer.Instance)
  {
  }

  public BytecodeCompiler(IBytecodeOptimizer optimizer)
  {
    this.optimizer = optimizer;
  }

  public ValueTask<ICompiledScript> CompileAsync(ScriptDeclaration declaration, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
