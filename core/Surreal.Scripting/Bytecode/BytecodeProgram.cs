using Surreal.Assets;
using Surreal.IO;
using Surreal.IO.Binary;

namespace Surreal.Scripting.Bytecode;

/// <summary>A <see cref="ICompiledScript"/> in the form of bytecode.</summary>
public sealed record BytecodeProgram : ICompiledScript
{
  public Memory<byte> Preamble  { get; init; }
  public Memory<byte> Resources { get; init; }
  public Memory<byte> Code      { get; init; }

  /// <summary>The <see cref="BinarySerializer"/> for <see cref="BytecodeProgram"/>s.</summary>
  [BinarySerializer(typeof(BytecodeProgram))]
  private sealed class BytecodeProgramSerializer : BinarySerializer<BytecodeProgram>
  {
    public override async ValueTask SerializeAsync(BytecodeProgram value, IBinaryWriter writer, IBinarySerializationContext context, CancellationToken cancellationToken = default)
    {
      await writer.WriteSpanAsync(stackalloc byte[] { 0x73, 0x73, 0x37, 0x37 }, cancellationToken);

      await writer.WriteMemoryAsync(value.Preamble, cancellationToken);
      await writer.WriteMemoryAsync(value.Resources, cancellationToken);
      await writer.WriteMemoryAsync(value.Code, cancellationToken);
    }

    public override ValueTask<BytecodeProgram> DeserializeAsync(IBinaryReader reader, IBinarySerializationContext context, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }
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
