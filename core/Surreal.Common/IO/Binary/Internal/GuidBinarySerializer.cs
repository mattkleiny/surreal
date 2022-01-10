using System.Buffers;

namespace Surreal.IO.Binary.Internal;

[BinarySerializer(typeof(Guid))]
public sealed class GuidBinarySerializer : BinarySerializer<Guid>
{
  public override async ValueTask SerializeAsync(Guid value, IBinaryWriter writer, IBinarySerializationContext context, CancellationToken cancellationToken = default)
  {
    var buffer = value.ToByteArray();

    await writer.WriteSpanAsync(buffer, cancellationToken);
  }

  public override async ValueTask<Guid> DeserializeAsync(IBinaryReader reader, IBinarySerializationContext context, CancellationToken cancellationToken = default)
  {
    var buffer = ArrayPool<byte>.Shared.Rent(4);

    try
    {
      await reader.ReadMemoryAsync(buffer, cancellationToken);

      return new Guid(buffer);
    }
    finally
    {
      ArrayPool<byte>.Shared.Return(buffer);
    }
  }
}
