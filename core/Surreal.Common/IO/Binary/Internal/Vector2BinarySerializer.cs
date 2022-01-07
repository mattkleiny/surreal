﻿namespace Surreal.IO.Binary.Internal;

[BinarySerializer(typeof(Vector2))]
public sealed class Vector2BinarySerializer : BinarySerializer<Vector2>
{
  public override async ValueTask SerializeAsync(Vector2 value, IBinaryWriter writer, IBinarySerializationContext context, CancellationToken cancellationToken = default)
  {
    await writer.WriteFloatAsync(value.X, cancellationToken);
    await writer.WriteFloatAsync(value.Y, cancellationToken);
  }

  public override async ValueTask<Vector2> DeserializeAsync(IBinaryReader reader, IBinarySerializationContext context, CancellationToken cancellationToken = default)
  {
    var x = await reader.ReadFloatAsync(cancellationToken);
    var y = await reader.ReadFloatAsync(cancellationToken);

    return new Vector2(x, y);
  }
}