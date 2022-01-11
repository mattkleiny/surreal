using Surreal.Assets;
using Surreal.IO;
using Surreal.IO.Binary;

namespace Surreal.Scripting.Bytecode;

/// <summary>Different kinds of <see cref="BytecodeInstruction"/>s.</summary>
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
  IInvoke,
  /// <summary>Directly yield control back to the execution thread (for spin waits).</summary>
  Yield,
}

/// <summary>Encapsulates a single bytecode instruction.</summary>
public readonly record struct BytecodeInstruction(InstructionType Type);

/// <summary>A <see cref="ICompiledScript"/> in the form of bytecode.</summary>
public sealed record BytecodeProgram : ICompiledScript
{
  public ImmutableList<BytecodeInstruction> Instructions { get; init; } = ImmutableList<BytecodeInstruction>.Empty;
}

/// <summary>The <see cref="BinarySerializer"/> for <see cref="BytecodeProgram"/>s.</summary>
[BinarySerializer(typeof(BytecodeProgram))]
public sealed class BytecodeProgramSerializer : BinarySerializer<BytecodeProgram>
{
  public override async ValueTask SerializeAsync(BytecodeProgram value, IBinaryWriter writer, CancellationToken cancellationToken = default)
  {
    await writer.WriteSpanAsync(stackalloc byte[] { 0x73, 0x37, 0x73, 0x37 }, cancellationToken);

    throw new NotImplementedException();
  }

  public override async ValueTask<BytecodeProgram> DeserializeAsync(IBinaryReader reader, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="BytecodeProgram"/>s.</summary>
public sealed class BytecodeProgramLoader : AssetLoader<BytecodeProgram>
{
  public override async ValueTask<BytecodeProgram> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken = default)
  {
    return await context.Path.DeserializeBinaryAsync<BytecodeProgram>(cancellationToken);
  }
}
