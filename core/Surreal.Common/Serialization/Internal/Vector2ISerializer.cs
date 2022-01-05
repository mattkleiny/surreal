using Surreal.Mathematics;

namespace Surreal.Serialization.Internal;

[Serializer(typeof(Vector2I))]
public sealed class Vector2ISerializer : Serializer<Vector2I>
{
  public override async ValueTask SerializeAsync(Vector2I value, ISerializationWriter writer, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    await writer.WriteIntAsync(value.X, cancellationToken);
    await writer.WriteIntAsync(value.Y, cancellationToken);
  }

  public override async ValueTask<Vector2I> DeserializeAsync(ISerializationReader reader, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    var x = await reader.ReadIntAsync(cancellationToken);
    var y = await reader.ReadIntAsync(cancellationToken);

    return new Vector2I(x, y);
  }
}
