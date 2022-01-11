namespace Surreal.IO.Binary.Internal;

[BinarySerializer(typeof(Vector4))]
public sealed class Vector4BinarySerializer : BinarySerializer<Vector4>
{
  public override async ValueTask SerializeAsync(Vector4 value, IBinaryWriter writer, CancellationToken cancellationToken = default)
  {
    await writer.WriteFloatAsync(value.X, cancellationToken);
    await writer.WriteFloatAsync(value.Y, cancellationToken);
    await writer.WriteFloatAsync(value.Z, cancellationToken);
    await writer.WriteFloatAsync(value.W, cancellationToken);
  }

  public override async ValueTask<Vector4> DeserializeAsync(IBinaryReader reader, CancellationToken cancellationToken = default)
  {
    var x = await reader.ReadFloatAsync(cancellationToken);
    var y = await reader.ReadFloatAsync(cancellationToken);
    var z = await reader.ReadFloatAsync(cancellationToken);
    var w = await reader.ReadFloatAsync(cancellationToken);

    return new Vector4(x, y, z, w);
  }
}
