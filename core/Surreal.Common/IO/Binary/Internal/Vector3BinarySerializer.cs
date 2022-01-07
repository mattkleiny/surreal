namespace Surreal.IO.Binary.Internal;

[BinarySerializer(typeof(Vector3))]
public sealed class Vector3BinarySerializer : BinarySerializer<Vector3>
{
  public override async ValueTask SerializeAsync(Vector3 value, IBinaryWriter writer, IBinarySerializationContext context, CancellationToken cancellationToken = default)
  {
    await writer.WriteFloatAsync(value.X, cancellationToken);
    await writer.WriteFloatAsync(value.Y, cancellationToken);
    await writer.WriteFloatAsync(value.Z, cancellationToken);
  }

  public override async ValueTask<Vector3> DeserializeAsync(IBinaryReader reader, IBinarySerializationContext context, CancellationToken cancellationToken = default)
  {
    var x = await reader.ReadFloatAsync(cancellationToken);
    var y = await reader.ReadFloatAsync(cancellationToken);
    var z = await reader.ReadFloatAsync(cancellationToken);

    return new Vector3(x, y, z);
  }
}
