using System.Buffers;
using Surreal.Utilities;

namespace Surreal.IO.Binary.Internal;

[BinarySerializer(typeof(Guid))]
public sealed class GuidBinarySerializer : BinarySerializer<Guid>
{
  public override async ValueTask SerializeAsync(Guid value, IBinaryWriter writer, CancellationToken cancellationToken = default)
  {
    await writer.WriteSpanAsync(Spans.AsSpan(ref value), cancellationToken);
  }

  public override async ValueTask<Guid> DeserializeAsync(IBinaryReader reader, CancellationToken cancellationToken = default)
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
