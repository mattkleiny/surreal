namespace Surreal.Scripting.Bytecode;

/// <summary>
/// A virtual machine for bytecode execution.
/// </summary>
public sealed class BytecodeVirtualMachine
{
  /// <summary>
  /// The intrinsic functions available to the virtual machine.
  /// </summary>
  public List<IntrinsicFunction> IntrinsicFunctions { get; } = new();

  /// <summary>
  /// The extrinsic functions available to the virtual machine.
  /// </summary>
  public List<ExtrinsicFunction> ExtrinsicFunctions { get; } = new();

  /// <summary>
  /// Executes a <see cref="BytecodeProgram"/>.
  /// </summary>
  public void Execute(BytecodeProgram program)
  {
    foreach (var instruction in program.Instructions)
    {
      Execute(instruction);
    }
  }

  /// <summary>
  /// Executes a single <see cref="BytecodeInstruction"/>.
  /// </summary>
  public void Execute(BytecodeInstruction instruction)
  {
    throw new NotImplementedException();
  }

  /// <summary>
  /// A function that is available for intrinsic execution in the runtime.
  /// </summary>
  public sealed record IntrinsicFunction(string Name, Delegate Body);

  /// <summary>
  /// An extrinsic function that is available across 'FFI' boundaries.
  /// </summary>
  public sealed record ExtrinsicFunction(string Name, Delegate Body);
}
