using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Scripting.VirtualMachine;

/// <summary>
/// Different kinds of <see cref="BytecodeInstruction"/>s.
/// </summary>
public enum InstructionType : ushort
{
  /// <summary>A standard no-op</summary>
  Nop = 0,
  /// <summary>Moves a value to/from a register.</summary>
  Mov,
  /// <summary>Pushes a value from the stack.</summary>
  Push,
  /// <summary>Pops a value from the stack.</summary>
  Pop,
  /// <summary>Invokes a function extrinsically</summary>
  EInvoke,
  /// <summary>Invokes a function intrinsically</summary>
  Invoke,
  /// <summary>Directly yield control back to the execution thread (for spin waits).</summary>
  Yield,
}

/// <summary>
/// Encapsulates a single bytecode instruction.
/// </summary>
public readonly record struct BytecodeInstruction(InstructionType Type);

/// <summary>
/// A <see cref="ICompiledScript"/> in the form of bytecode that can be executed by our <see cref="BytecodeVirtualMachine"/>.
/// </summary>
public sealed record BytecodeProgram(string Path) : ICompiledScript
{
  /// <summary>
  /// The instructions that make up this program.
  /// </summary>
  public ImmutableList<BytecodeInstruction> Instructions { get; init; } = ImmutableList<BytecodeInstruction>.Empty;
}

/// <summary>
/// The <see cref="AssetLoader{T}"/> for <see cref="BytecodeProgram"/>s.
/// </summary>
public sealed class BytecodeProgramLoader : AssetLoader<BytecodeProgram>
{
  public override Task<BytecodeProgram> LoadAsync(AssetContext context, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}

/// <summary>
/// The <see cref="BinarySerializer{T}"/> for <see cref="BytecodeProgram"/>s.
/// </summary>
public sealed class BytecodeProgramSerializer : BinarySerializer<BytecodeProgram>
{
  public override ValueTask SerializeAsync(BytecodeProgram value, FastBinaryWriter writer, CancellationToken cancellationToken = default)
  {
    writer.WriteBytes(stackalloc byte[] { 0x73, 0x37, 0x73, 0x37 });

    throw new NotImplementedException();
  }

  public override ValueTask<BytecodeProgram> DeserializeAsync(FastBinaryReader reader, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
