using Surreal.Mathematics;

namespace Surreal.IO.Serialization.Internal;

[Serializer(typeof(Vector3I))]
public sealed class Vector3ISerializer : BinarySerializer<Vector3I>
{
  public override async ValueTask SerializeAsync(Vector3I value, IBinaryWriter writer, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    await writer.WriteIntAsync(value.X, cancellationToken);
    await writer.WriteIntAsync(value.Y, cancellationToken);
    await writer.WriteIntAsync(value.Z, cancellationToken);
  }

  public override async ValueTask<Vector3I> DeserializeAsync(IBinaryReader reader, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    var x = await reader.ReadIntAsync(cancellationToken);
    var y = await reader.ReadIntAsync(cancellationToken);
    var z = await reader.ReadIntAsync(cancellationToken);

    return new Vector3I(x, y, z);
  }
}
