using Surreal.Mathematics;

namespace Surreal.Serialization.Internal;

[Serializer(typeof(VolumeI))]
public sealed class VolumeISerializer : Serializer<VolumeI>
{
  public override async ValueTask SerializeAsync(VolumeI value, ISerializationWriter writer, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    await writer.WriteIntAsync(value.Width, cancellationToken);
    await writer.WriteIntAsync(value.Height, cancellationToken);
    await writer.WriteIntAsync(value.Depth, cancellationToken);
  }

  public override async ValueTask<VolumeI> DeserializeAsync(ISerializationReader reader, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    var width  = await reader.ReadIntAsync(cancellationToken);
    var height = await reader.ReadIntAsync(cancellationToken);
    var depth  = await reader.ReadIntAsync(cancellationToken);

    return new VolumeI(width, height, depth);
  }
}
