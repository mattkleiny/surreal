using Surreal.Mathematics;

namespace Surreal.IO.Binary.Internal;

[BinarySerializer(typeof(VolumeI))]
public sealed class VolumeIBinarySerializer : BinarySerializer<VolumeI>
{
  public override async ValueTask SerializeAsync(VolumeI value, IBinaryWriter writer, IBinarySerializationContext context, CancellationToken cancellationToken = default)
  {
    await writer.WriteIntAsync(value.Width, cancellationToken);
    await writer.WriteIntAsync(value.Height, cancellationToken);
    await writer.WriteIntAsync(value.Depth, cancellationToken);
  }

  public override async ValueTask<VolumeI> DeserializeAsync(IBinaryReader reader, IBinarySerializationContext context, CancellationToken cancellationToken = default)
  {
    var width  = await reader.ReadIntAsync(cancellationToken);
    var height = await reader.ReadIntAsync(cancellationToken);
    var depth  = await reader.ReadIntAsync(cancellationToken);

    return new VolumeI(width, height, depth);
  }
}
