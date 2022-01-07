using Surreal.Mathematics;

namespace Surreal.IO.Binary.Internal;

[BinarySerializer(typeof(BoundingBox))]
public sealed class BoundingBoxBinarySerializer : BinarySerializer<BoundingBox>
{
  public override async ValueTask SerializeAsync(BoundingBox value, IBinaryWriter writer, IBinarySerializationContext context, CancellationToken cancellationToken = default)
  {
    await context.SerializeAsync(value.Min, writer, cancellationToken);
    await context.SerializeAsync(value.Max, writer, cancellationToken);
  }

  public override async ValueTask<BoundingBox> DeserializeAsync(IBinaryReader reader, IBinarySerializationContext context, CancellationToken cancellationToken = default)
  {
    var min = await context.DeserializeAsync<Vector3>(reader, cancellationToken);
    var max = await context.DeserializeAsync<Vector3>(reader, cancellationToken);

    return new BoundingBox(min, max);
  }
}
