﻿using Surreal.Mathematics;

namespace Surreal.Serialization.Internal;

[Serializer(typeof(IntRange))]
public sealed class IntRangeSerializer : Serializer<IntRange>
{
  public override async ValueTask SerializeAsync(IntRange value, ISerializationWriter writer, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    await writer.WriteIntAsync(value.Min, cancellationToken);
    await writer.WriteIntAsync(value.Max, cancellationToken);
  }

  public override async ValueTask<IntRange> DeserializeAsync(ISerializationReader reader, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    var min = await reader.ReadIntAsync(cancellationToken);
    var max = await reader.ReadIntAsync(cancellationToken);

    return new IntRange(min, max);
  }
}
