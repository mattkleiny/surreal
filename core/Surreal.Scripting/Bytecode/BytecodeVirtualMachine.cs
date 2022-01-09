namespace Surreal.Scripting.Bytecode;

/// <summary>A virtual machine for bytecode execution.</summary>
public sealed class BytecodeVirtualMachine
{
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
}
