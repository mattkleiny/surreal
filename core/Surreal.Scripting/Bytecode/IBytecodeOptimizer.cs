namespace Surreal.Scripting.Bytecode;

/// <summary>An optimization strategy for the <see cref="BytecodeCompiler"/>.</summary>
public interface IBytecodeOptimizer
{
}

/// <summary>A no-op <see cref="IBytecodeOptimizer"/> implementation.</summary>
internal sealed class NullBytecodeOptimizer : IBytecodeOptimizer
{
  public static NullBytecodeOptimizer Instance { get; } = new();
}
