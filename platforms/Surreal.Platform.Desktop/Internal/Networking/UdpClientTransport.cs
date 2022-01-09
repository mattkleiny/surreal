﻿using Surreal.Networking.Transports;

namespace Surreal.Internal.Networking;

internal sealed class UdpClientTransport : IClientTransport
{
  public TransportType Type => TransportType.Throughput;

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
