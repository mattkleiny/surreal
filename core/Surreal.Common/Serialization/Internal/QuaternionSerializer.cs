namespace Surreal.Serialization.Internal;

[Serializer(typeof(Quaternion))]
public sealed class QuaternionSerializer : BinarySerializer<Quaternion>
{
  public override async ValueTask SerializeAsync(Quaternion value, IBinaryWriter writer, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    await writer.WriteFloatAsync(value.X, cancellationToken);
    await writer.WriteFloatAsync(value.Y, cancellationToken);
    await writer.WriteFloatAsync(value.Z, cancellationToken);
    await writer.WriteFloatAsync(value.W, cancellationToken);
  }

  public override async ValueTask<Quaternion> DeserializeAsync(IBinaryReader reader, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    var x = await reader.ReadFloatAsync(cancellationToken);
    var y = await reader.ReadFloatAsync(cancellationToken);
    var z = await reader.ReadFloatAsync(cancellationToken);
    var w = await reader.ReadFloatAsync(cancellationToken);

    return new Quaternion(x, y, z, w);
  }
}
