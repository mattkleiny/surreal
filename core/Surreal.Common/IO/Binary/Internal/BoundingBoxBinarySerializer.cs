using Surreal.Mathematics;

namespace Surreal.IO.Binary.Internal;

[BinarySerializer(typeof(BoundingBox))]
public sealed class BoundingBoxBinarySerializer : BinarySerializer<BoundingBox>
{
  public override async ValueTask SerializeAsync(BoundingBox value, IBinaryWriter writer, CancellationToken cancellationToken = default)
  {
    await SerializeAsync(value.Min, writer, cancellationToken);
    await SerializeAsync(value.Max, writer, cancellationToken);
  }

  public override async ValueTask<BoundingBox> DeserializeAsync(IBinaryReader reader, CancellationToken cancellationToken = default)
  {
    var min = await DeserializeAsync<Vector3>(reader, cancellationToken);
    var max = await DeserializeAsync<Vector3>(reader, cancellationToken);

    return new BoundingBox(min, max);
  }
}
