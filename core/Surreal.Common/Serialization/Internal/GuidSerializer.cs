namespace Surreal.Serialization.Internal;

[Serializer(typeof(Guid))]
public sealed class GuidSerializer : BinarySerializer<Guid>
{
  public override async ValueTask SerializeAsync(Guid value, IBinaryWriter writer, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    var buffer = value.ToByteArray();

    await writer.WriteSpanAsync(buffer, cancellationToken);
  }

  public override async ValueTask<Guid> DeserializeAsync(IBinaryReader reader, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    var buffer = await reader.ReadBufferAsync(cancellationToken);

    return new Guid(buffer.ToArray());
  }
}
