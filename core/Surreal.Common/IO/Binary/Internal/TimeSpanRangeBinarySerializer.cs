using Surreal.Mathematics;

namespace Surreal.IO.Binary.Internal;

[BinarySerializer(typeof(TimeSpanRange))]
internal sealed class TimeSpanRangeBinarySerializer : BinarySerializer<TimeSpanRange>
{
  public override async ValueTask SerializeAsync(TimeSpanRange value, IBinaryWriter writer, CancellationToken cancellationToken = default)
  {
    await SerializeAsync(value.Min, writer, cancellationToken);
    await SerializeAsync(value.Max, writer, cancellationToken);
  }

  public override async ValueTask<TimeSpanRange> DeserializeAsync(IBinaryReader reader, CancellationToken cancellationToken = default)
  {
    var min = await DeserializeAsync<TimeSpan>(reader, cancellationToken);
    var max = await DeserializeAsync<TimeSpan>(reader, cancellationToken);

    return new TimeSpanRange(min, max);
  }
}
