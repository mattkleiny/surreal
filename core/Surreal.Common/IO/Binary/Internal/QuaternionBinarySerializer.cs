namespace Surreal.IO.Binary.Internal;

[BinarySerializer(typeof(Quaternion))]
public sealed class QuaternionBinarySerializer : BinarySerializer<Quaternion>
{
  public override async ValueTask SerializeAsync(Quaternion value, IBinaryWriter writer, CancellationToken cancellationToken = default)
  {
    await writer.WriteFloatAsync(value.X, cancellationToken);
    await writer.WriteFloatAsync(value.Y, cancellationToken);
    await writer.WriteFloatAsync(value.Z, cancellationToken);
    await writer.WriteFloatAsync(value.W, cancellationToken);
  }

  public override async ValueTask<Quaternion> DeserializeAsync(IBinaryReader reader, CancellationToken cancellationToken = default)
  {
    var x = await reader.ReadFloatAsync(cancellationToken);
    var y = await reader.ReadFloatAsync(cancellationToken);
    var z = await reader.ReadFloatAsync(cancellationToken);
    var w = await reader.ReadFloatAsync(cancellationToken);

    return new Quaternion(x, y, z, w);
  }
}
