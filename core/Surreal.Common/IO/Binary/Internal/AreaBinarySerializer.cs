﻿using Surreal.Mathematics;

namespace Surreal.IO.Binary.Internal;

[BinarySerializer(typeof(Area))]
internal sealed class AreaBinarySerializer : BinarySerializer<Area>
{
  public override async ValueTask SerializeAsync(Area value, IBinaryWriter writer, CancellationToken cancellationToken = default)
  {
    await writer.WriteFloatAsync(value.Width, cancellationToken);
    await writer.WriteFloatAsync(value.Height, cancellationToken);
  }

  public override async ValueTask<Area> DeserializeAsync(IBinaryReader reader, CancellationToken cancellationToken = default)
  {
    var width = await reader.ReadFloatAsync(cancellationToken);
    var height = await reader.ReadFloatAsync(cancellationToken);

    return new Area(width, height);
  }
}
