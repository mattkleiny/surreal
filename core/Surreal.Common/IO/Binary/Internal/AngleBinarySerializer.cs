using Surreal.Mathematics;

namespace Surreal.IO.Binary.Internal;

[BinarySerializer(typeof(Angle))]
public sealed class AngleBinarySerializer : BinarySerializer<Angle>
{
  public override async ValueTask SerializeAsync(Angle value, IBinaryWriter writer, CancellationToken cancellationToken = default)
  {
    await writer.WriteFloatAsync(value.Radians, cancellationToken);
  }

  public override async ValueTask<Angle> DeserializeAsync(IBinaryReader reader, CancellationToken cancellationToken = default)
  {
    var radians = await reader.ReadFloatAsync(cancellationToken);

    return new Angle(radians);
  }
}
