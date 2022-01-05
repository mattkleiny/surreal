using Surreal.Mathematics;

namespace Surreal.Serialization.Internal;

[Serializer(typeof(FloatRange))]
public sealed class FloatRangeSerializer : Serializer<FloatRange>
{
  public override async ValueTask SerializeAsync(FloatRange value, ISerializationWriter writer, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    await writer.WriteFloatAsync(value.Min, cancellationToken);
    await writer.WriteFloatAsync(value.Max, cancellationToken);
  }

  public override async ValueTask<FloatRange> DeserializeAsync(ISerializationReader reader, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    var min = await reader.ReadFloatAsync(cancellationToken);
    var max = await reader.ReadFloatAsync(cancellationToken);

    return new FloatRange(min, max);
  }
}
