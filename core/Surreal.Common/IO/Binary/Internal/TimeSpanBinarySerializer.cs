namespace Surreal.IO.Binary.Internal;

[BinarySerializer(typeof(TimeSpan))]
public sealed class TimeSpanBinarySerializer : BinarySerializer<TimeSpan>
{
  public override async ValueTask SerializeAsync(TimeSpan value, IBinaryWriter writer, CancellationToken cancellationToken = default)
  {
    await writer.WriteLongAsync(value.Ticks, cancellationToken);
  }

  public override async ValueTask<TimeSpan> DeserializeAsync(IBinaryReader reader, CancellationToken cancellationToken = default)
  {
    return new TimeSpan(await reader.ReadLongAsync(cancellationToken));
  }
}
