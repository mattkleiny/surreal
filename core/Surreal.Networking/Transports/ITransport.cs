namespace Surreal.Networking.Transports;

/// <summary>
/// Different types of <see cref="ITransport"/> supported by the engine.
/// </summary>
public enum TransportType
{
  Throughput,
  Reliability,
}

/// <summary>
/// Options for creation of <see cref="ITransport"/>s.
/// </summary>
public sealed record TransportOptions
{
  public static TransportOptions Default { get; } = new();

  /// <summary>
  /// The type of transport to create.
  /// </summary>
  public TransportType Type { get; init; } = TransportType.Reliability;

  /// <summary>
  /// The desired port to use; if not specified, a random port will be used.
  /// </summary>
  public Optional<int> Port { get; init; }
}

/// <summary>
/// Transport for standard purpose network I/O.
/// </summary>
public interface ITransport : IDisposable, IAsyncDisposable
{
  /// <summary>
  /// The type of transport.
  /// </summary>
  TransportType Type { get; }

  /// <summary>
  /// Sends the given buffer to the remote endpoint.
  /// </summary>
  ValueTask SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);

  /// <summary>
  /// Receives data from the remote endpoint into the given buffer.
  /// </summary>
  ValueTask ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
}
