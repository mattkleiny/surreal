using Surreal.Mathematics;

namespace Surreal.IO.Serialization.Internal;

[Serializer(typeof(Seed))]
public sealed class SeedSerializer : BinarySerializer<Seed>
{
  public override async ValueTask SerializeAsync(Seed value, IBinaryWriter writer, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    await writer.WriteIntAsync(value.Value, cancellationToken);
  }

  public override async ValueTask<Seed> DeserializeAsync(IBinaryReader reader, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    var value = await reader.ReadIntAsync(cancellationToken);

    return new Seed(value);
  }
}
