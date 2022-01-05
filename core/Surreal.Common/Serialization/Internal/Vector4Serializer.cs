namespace Surreal.Serialization.Internal;

[Serializer(typeof(Vector4))]
public sealed class Vector4Serializer : Serializer<Vector4>
{
  public override async ValueTask SerializeAsync(Vector4 value, ISerializationWriter writer, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    await writer.WriteFloatAsync(value.X, cancellationToken);
    await writer.WriteFloatAsync(value.Y, cancellationToken);
    await writer.WriteFloatAsync(value.Z, cancellationToken);
    await writer.WriteFloatAsync(value.W, cancellationToken);
  }

  public override async ValueTask<Vector4> DeserializeAsync(ISerializationReader reader, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    var x = await reader.ReadFloatAsync(cancellationToken);
    var y = await reader.ReadFloatAsync(cancellationToken);
    var z = await reader.ReadFloatAsync(cancellationToken);
    var w = await reader.ReadFloatAsync(cancellationToken);

    return new Vector4(x, y, z, w);
  }
}
