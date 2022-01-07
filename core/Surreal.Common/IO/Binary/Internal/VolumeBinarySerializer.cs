using Surreal.Mathematics;

namespace Surreal.IO.Binary.Internal;

[BinarySerializer(typeof(Volume))]
public sealed class VolumeBinarySerializer : BinarySerializer<Volume>
{
  public override async ValueTask SerializeAsync(Volume value, IBinaryWriter writer, IBinarySerializationContext context, CancellationToken cancellationToken = default)
  {
    await writer.WriteFloatAsync(value.Width, cancellationToken);
    await writer.WriteFloatAsync(value.Height, cancellationToken);
    await writer.WriteFloatAsync(value.Depth, cancellationToken);
  }

  public override async ValueTask<Volume> DeserializeAsync(IBinaryReader reader, IBinarySerializationContext context, CancellationToken cancellationToken = default)
  {
    var width  = await reader.ReadFloatAsync(cancellationToken);
    var height = await reader.ReadFloatAsync(cancellationToken);
    var depth  = await reader.ReadFloatAsync(cancellationToken);

    return new Volume(width, height, depth);
  }
}
