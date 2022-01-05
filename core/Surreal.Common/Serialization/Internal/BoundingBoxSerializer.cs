﻿using Surreal.Mathematics;

namespace Surreal.Serialization.Internal;

[Serializer(typeof(BoundingBox))]
public sealed class BoundingBoxSerializer : Serializer<BoundingBox>
{
  public override async ValueTask SerializeAsync(BoundingBox value, ISerializationWriter writer, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    await context.SerializeAsync(value.Min, writer, cancellationToken);
    await context.SerializeAsync(value.Max, writer, cancellationToken);
  }

  public override async ValueTask<BoundingBox> DeserializeAsync(ISerializationReader reader, ISerializationContext context, CancellationToken cancellationToken = default)
  {
    var min = await context.DeserializeAsync<Vector3>(reader, cancellationToken);
    var max = await context.DeserializeAsync<Vector3>(reader, cancellationToken);

    return new BoundingBox(min, max);
  }
}
