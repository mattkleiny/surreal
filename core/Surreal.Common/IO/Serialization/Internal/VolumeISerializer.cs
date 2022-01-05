using Surreal.Mathematics;

namespace Surreal.IO.Serialization.Internal;

[Serializer(typeof(VolumeI))]
public sealed class VolumeISerializer : BinarySerializer<VolumeI>
{
  public override async ValueTask SerializeAsync(VolumeI value, IBinaryWriter writer, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    await writer.WriteIntAsync(value.Width, cancellationToken);
    await writer.WriteIntAsync(value.Height, cancellationToken);
    await writer.WriteIntAsync(value.Depth, cancellationToken);
  }

  public override async ValueTask<VolumeI> DeserializeAsync(IBinaryReader reader, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    var width  = await reader.ReadIntAsync(cancellationToken);
    var height = await reader.ReadIntAsync(cancellationToken);
    var depth  = await reader.ReadIntAsync(cancellationToken);

    return new VolumeI(width, height, depth);
  }
}
