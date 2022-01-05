using Surreal.Mathematics;

namespace Surreal.Serialization.Internal;

[Serializer(typeof(Angle))]
public sealed class AngleSerializer : Serializer<Angle>
{
  public override async ValueTask SerializeAsync(Angle value, ISerializationWriter writer, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    await writer.WriteFloatAsync(value.Radians, cancellationToken);
  }

  public override async ValueTask<Angle> DeserializeAsync(ISerializationReader reader, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    var radians = await reader.ReadFloatAsync(cancellationToken);

    return new Angle(radians);
  }
}
