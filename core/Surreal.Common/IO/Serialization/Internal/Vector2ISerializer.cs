using Surreal.Mathematics;

namespace Surreal.IO.Serialization.Internal;

[Serializer(typeof(Vector2I))]
public sealed class Vector2ISerializer : BinarySerializer<Vector2I>
{
  public override async ValueTask SerializeAsync(Vector2I value, IBinaryWriter writer, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    await writer.WriteIntAsync(value.X, cancellationToken);
    await writer.WriteIntAsync(value.Y, cancellationToken);
  }

  public override async ValueTask<Vector2I> DeserializeAsync(IBinaryReader reader, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    var x = await reader.ReadIntAsync(cancellationToken);
    var y = await reader.ReadIntAsync(cancellationToken);

    return new Vector2I(x, y);
  }
}
