namespace Surreal.Scripting.Bytecode;

/// <summary>A virtual machine for bytecode execution.</summary>
public sealed class BytecodeVirtualMachine
{
  public List<IntrinsicFunction> IntrinsicFunctions { get; } = new();
  public List<ExtrinsicFunction> ExtrinsicFunctions { get; } = new();

  /// <summary>Executes a <see cref="BytecodeProgram"/>.</summary>
  public async ValueTask ExecuteAsync(BytecodeProgram program, CancellationToken cancellationToken = default)
  {
    foreach (var instruction in program.Instructions)
    {
      await ExecuteAsync(instruction, cancellationToken);
    }
  }

  /// <summary>Executes a single <see cref="BytecodeInstruction"/>.</summary>
  public ValueTask ExecuteAsync(BytecodeInstruction instruction, CancellationToken cancellationToken = default)
  {
    // TODO: implement me

    return ValueTask.CompletedTask;
  }

  /// <summary>A function that is available for intrinsic execution in the runtime.</summary>
  public sealed record IntrinsicFunction(string Name, Delegate Body);

  /// <summary>An extrinsic function that is available for invoked execution in the runtime.</summary>
  public sealed record ExtrinsicFunction(string Name, Delegate Body);
}
