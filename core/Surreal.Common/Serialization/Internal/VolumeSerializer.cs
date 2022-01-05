using Surreal.Mathematics;

namespace Surreal.Serialization.Internal;

[Serializer(typeof(Volume))]
public sealed class VolumeSerializer : Serializer<Volume>
{
  public override async ValueTask SerializeAsync(Volume value, ISerializationWriter writer, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    await writer.WriteFloatAsync(value.Width, cancellationToken);
    await writer.WriteFloatAsync(value.Height, cancellationToken);
    await writer.WriteFloatAsync(value.Depth, cancellationToken);
  }

  public override async ValueTask<Volume> DeserializeAsync(ISerializationReader reader, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    var width  = await reader.ReadFloatAsync(cancellationToken);
    var height = await reader.ReadFloatAsync(cancellationToken);
    var depth  = await reader.ReadFloatAsync(cancellationToken);

    return new Volume(width, height, depth);
  }
}
