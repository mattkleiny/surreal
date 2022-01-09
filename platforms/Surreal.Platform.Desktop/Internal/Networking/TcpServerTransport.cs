using Surreal.Networking.Transports;

namespace Surreal.Internal.Networking;

internal sealed class TcpServerTransport : IServerTransport
{
  public TransportType Type => TransportType.Reliability;

  public ValueTask SendAsync(ReadOnlySpan<byte> buffer, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public ValueTask ReceiveAsync(Span<byte> buffer, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public void Dispose()
  {
    throw new NotImplementedException();
  }

  public ValueTask DisposeAsync()
  {
    throw new NotImplementedException();
  }
}
