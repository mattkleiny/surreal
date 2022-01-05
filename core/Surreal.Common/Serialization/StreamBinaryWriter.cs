namespace Surreal.Serialization;

/// <summary>A <see cref="IBinaryWriter"/> that writes to the given <see cref="Stream"/>.</summary>
public sealed class StreamBinaryWriter : IBinaryWriter, IDisposable, IAsyncDisposable
{
  private readonly Stream stream;

  public StreamBinaryWriter(Stream stream)
  {
    this.stream = stream;
  }

  public ValueTask WriteSpanAsync(ReadOnlySpan<byte> value, CancellationToken cancellationToken = default)
  {
    stream.Write(value);

    return ValueTask.CompletedTask;
  }

  public ValueTask WriteMemoryAsync(ReadOnlyMemory<byte> value, CancellationToken cancellationToken = default)
  {
    return stream.WriteAsync(value, cancellationToken);
  }

  public void Dispose()
  {
    stream.Dispose();
  }

  public ValueTask DisposeAsync()
  {
    return stream.DisposeAsync();
  }
}
