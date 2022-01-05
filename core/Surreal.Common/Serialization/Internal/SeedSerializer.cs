using Surreal.Mathematics;

namespace Surreal.Serialization.Internal;

[Serializer(typeof(Seed))]
public sealed class SeedSerializer : Serializer<Seed>
{
  public override async ValueTask SerializeAsync(Seed value, ISerializationWriter writer, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    await writer.WriteIntAsync(value.Value, cancellationToken);
  }

  public override async ValueTask<Seed> DeserializeAsync(ISerializationReader reader, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    var value = await reader.ReadIntAsync(cancellationToken);

    return new Seed(value);
  }
}
