namespace Surreal.Serialization.Internal;

[Serializer(typeof(Vector2))]
public sealed class Vector2Serializer : BinarySerializer<Vector2>
{
  public override async ValueTask SerializeAsync(Vector2 value, IBinaryWriter writer, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    await writer.WriteFloatAsync(value.X, cancellationToken);
    await writer.WriteFloatAsync(value.Y, cancellationToken);
  }

  public override async ValueTask<Vector2> DeserializeAsync(IBinaryReader reader, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    var x = await reader.ReadFloatAsync(cancellationToken);
    var y = await reader.ReadFloatAsync(cancellationToken);

    return new Vector2(x, y);
  }
}
