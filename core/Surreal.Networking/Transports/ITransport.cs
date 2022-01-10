namespace Surreal.Networking.Transports;

/// <summary>Different types of <see cref="ITransport"/> supported by the engine.</summary>
public enum TransportType
{
  Throughput,
  Reliability,
}

/// <summary>Options for creation of <see cref="ITransport"/>s.</summary>
public sealed record TransportOptions
{
  public static TransportOptions Default { get; } = new();

  public TransportType Type { get; init; } = TransportType.Reliability;
  public int           Port { get; init; } = 28005;
}

/// <summary>A factory for <see cref="ITransport"/>s.</summary>
public interface ITransportFactory
{
  IServerTransport CreateServerTransport(TransportOptions options);
  IClientTransport CreateClientTransport(TransportOptions options);
}

/// <summary>Transport for standard purpose network I/O.</summary>
public interface ITransport : IDisposable, IAsyncDisposable
{
  TransportType Type { get; }

  ValueTask SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);
  ValueTask ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
}

/// <summary>Client-side <see cref="ITransport"/>.</summary>
public interface IClientTransport : ITransport
{
  ValueTask ConnectToServerAsync();
}

/// <summary>Server-side <see cref="ITransport"/>.</summary>
public interface IServerTransport : ITransport
{
  ValueTask StartServerAsync();
}
