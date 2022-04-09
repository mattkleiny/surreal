using Surreal.Mathematics;

namespace Surreal.IO.Binary.Internal;

[BinarySerializer(typeof(IntRange))]
internal sealed class IntRangeBinarySerializer : BinarySerializer<IntRange>
{
  public override async ValueTask SerializeAsync(IntRange value, IBinaryWriter writer, CancellationToken cancellationToken = default)
  {
    await writer.WriteIntAsync(value.Min, cancellationToken);
    await writer.WriteIntAsync(value.Max, cancellationToken);
  }

  public override async ValueTask<IntRange> DeserializeAsync(IBinaryReader reader, CancellationToken cancellationToken = default)
  {
    var min = await reader.ReadIntAsync(cancellationToken);
    var max = await reader.ReadIntAsync(cancellationToken);

    return new IntRange(min, max);
  }
}
