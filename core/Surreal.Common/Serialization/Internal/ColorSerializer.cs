using Surreal.Mathematics;

namespace Surreal.Serialization.Internal;

[Serializer(typeof(Color))]
public sealed class ColorSerializer : Serializer<Color>
{
  public override async ValueTask SerializeAsync(Color value, ISerializationWriter writer, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    await writer.WriteFloatAsync(value.R, cancellationToken);
    await writer.WriteFloatAsync(value.G, cancellationToken);
    await writer.WriteFloatAsync(value.B, cancellationToken);
    await writer.WriteFloatAsync(value.A, cancellationToken);
  }

  public override async ValueTask<Color> DeserializeAsync(ISerializationReader reader, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    var r = await reader.ReadFloatAsync(cancellationToken);
    var g = await reader.ReadFloatAsync(cancellationToken);
    var b = await reader.ReadFloatAsync(cancellationToken);
    var a = await reader.ReadFloatAsync(cancellationToken);

    return new Color(r, g, b, a);
  }
}
