using Surreal.Mathematics;

namespace Surreal.IO.Binary.Internal;

[BinarySerializer(typeof(Point3))]
public sealed class Point3BinarySerializer : BinarySerializer<Point3>
{
  public override async ValueTask SerializeAsync(Point3 value, IBinaryWriter writer, CancellationToken cancellationToken = default)
  {
    await writer.WriteIntAsync(value.X, cancellationToken);
    await writer.WriteIntAsync(value.Y, cancellationToken);
    await writer.WriteIntAsync(value.Z, cancellationToken);
  }

  public override async ValueTask<Point3> DeserializeAsync(IBinaryReader reader, CancellationToken cancellationToken = default)
  {
    var x = await reader.ReadIntAsync(cancellationToken);
    var y = await reader.ReadIntAsync(cancellationToken);
    var z = await reader.ReadIntAsync(cancellationToken);

    return new Point3(x, y, z);
  }
}
