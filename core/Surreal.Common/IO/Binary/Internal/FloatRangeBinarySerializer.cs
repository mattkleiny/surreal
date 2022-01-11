using Surreal.Mathematics;

namespace Surreal.IO.Binary.Internal;

[BinarySerializer(typeof(FloatRange))]
public sealed class FloatRangeBinarySerializer : BinarySerializer<FloatRange>
{
  public override async ValueTask SerializeAsync(FloatRange value, IBinaryWriter writer, CancellationToken cancellationToken = default)
  {
    await writer.WriteFloatAsync(value.Min, cancellationToken);
    await writer.WriteFloatAsync(value.Max, cancellationToken);
  }

  public override async ValueTask<FloatRange> DeserializeAsync(IBinaryReader reader, CancellationToken cancellationToken = default)
  {
    var min = await reader.ReadFloatAsync(cancellationToken);
    var max = await reader.ReadFloatAsync(cancellationToken);

    return new FloatRange(min, max);
  }
}
