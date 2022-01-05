namespace Surreal.Serialization;

/// <summary>Allows reading values from a serialization stream.</summary>
public interface ISerializationReader
{
  ValueTask<bool>   ReadBoolAsync(CancellationToken cancellationToken = default);
  ValueTask<byte>   ReadByteAsync(CancellationToken cancellationToken = default);
  ValueTask<sbyte>  ReadSByteAsync(CancellationToken cancellationToken = default);
  ValueTask<short>  ReadShortAsync(CancellationToken cancellationToken = default);
  ValueTask<ushort> ReadUShortAsync(CancellationToken cancellationToken = default);
  ValueTask<int>    ReadIntAsync(CancellationToken cancellationToken = default);
  ValueTask<uint>   ReadUIntAsync(CancellationToken cancellationToken = default);
  ValueTask<long>   ReadLongAsync(CancellationToken cancellationToken = default);
  ValueTask<ulong>  ReadULongAsync(CancellationToken cancellationToken = default);
  ValueTask<float>  ReadFloatAsync(CancellationToken cancellationToken = default);
  ValueTask<double> ReadDoubleAsync(CancellationToken cancellationToken = default);
  ValueTask<string> ReadStringAsync(CancellationToken cancellationToken = default);

  ValueTask               ReadBufferAsync(Span<byte> buffer, CancellationToken cancellationToken = default);
  ValueTask<Memory<byte>> ReadBufferAsync(CancellationToken cancellationToken = default);
}
