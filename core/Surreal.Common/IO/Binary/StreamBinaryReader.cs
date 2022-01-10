namespace Surreal.IO.Binary;

/// <summary>A <see cref="IBinaryReader"/> that readers from the given <see cref="Stream"/>.</summary>
public sealed class StreamBinaryReader : IBinaryReader, IDisposable, IAsyncDisposable
{
  private readonly Stream stream;

  public StreamBinaryReader(Stream stream)
  {
    this.stream = stream;
  }

  public ValueTask<bool> ReadBoolAsync(CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public ValueTask<sbyte> ReadSByteAsync(CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public ValueTask<short> ReadShortAsync(CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public ValueTask<ushort> ReadUShortAsync(CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public ValueTask<int> ReadIntAsync(CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public ValueTask<uint> ReadUIntAsync(CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public ValueTask<long> ReadLongAsync(CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public ValueTask<ulong> ReadULongAsync(CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public ValueTask<float> ReadFloatAsync(CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public ValueTask<double> ReadDoubleAsync(CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public ValueTask<string> ReadStringAsync(CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public ValueTask ReadSpanAsync(Span<byte> buffer, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public ValueTask ReadMemoryAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
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
