using Surreal.Mathematics;

namespace Surreal.IO.Binary.Internal;

[BinarySerializer(typeof(Point2))]
public sealed class Point2BinarySerializer : BinarySerializer<Point2>
{
  public override async ValueTask SerializeAsync(Point2 value, IBinaryWriter writer, CancellationToken cancellationToken = default)
  {
    await writer.WriteIntAsync(value.X, cancellationToken);
    await writer.WriteIntAsync(value.Y, cancellationToken);
  }

  public override async ValueTask<Point2> DeserializeAsync(IBinaryReader reader, CancellationToken cancellationToken = default)
  {
    var x = await reader.ReadIntAsync(cancellationToken);
    var y = await reader.ReadIntAsync(cancellationToken);

    return new Point2(x, y);
  }
}
