using Surreal.Mathematics;

namespace Surreal.Serialization.Internal;

[Serializer(typeof(Rectangle))]
public sealed class RectangleSerializer : BinarySerializer<Rectangle>
{
  public override async ValueTask SerializeAsync(Rectangle value, IBinaryWriter writer, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    await writer.WriteFloatAsync(value.Left, cancellationToken);
    await writer.WriteFloatAsync(value.Top, cancellationToken);
    await writer.WriteFloatAsync(value.Right, cancellationToken);
    await writer.WriteFloatAsync(value.Bottom, cancellationToken);
  }

  public override async ValueTask<Rectangle> DeserializeAsync(IBinaryReader reader, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    var left   = await reader.ReadFloatAsync(cancellationToken);
    var top    = await reader.ReadFloatAsync(cancellationToken);
    var right  = await reader.ReadFloatAsync(cancellationToken);
    var bottom = await reader.ReadFloatAsync(cancellationToken);

    return new Rectangle(left, top, right, bottom);
  }
}
