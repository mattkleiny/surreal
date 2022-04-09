using Surreal.Mathematics;

namespace Surreal.IO.Binary.Internal;

[BinarySerializer(typeof(Seed))]
internal sealed class SeedBinarySerializer : BinarySerializer<Seed>
{
  public override async ValueTask SerializeAsync(Seed value, IBinaryWriter writer, CancellationToken cancellationToken = default)
  {
    await writer.WriteIntAsync(value.Value, cancellationToken);
  }

  public override async ValueTask<Seed> DeserializeAsync(IBinaryReader reader, CancellationToken cancellationToken = default)
  {
    var value = await reader.ReadIntAsync(cancellationToken);

    return new Seed(value);
  }
}
