using System.Net.Sockets;
using Surreal.Networking.Transports;

namespace Surreal.Internal.Networking;

internal sealed class UdpClientTransport : SocketTransport, IClientTransport
{
  public UdpClientTransport(TransportOptions options)
    : base(SocketType.Dgram)
  {
  }

  public TransportType Type => TransportType.Throughput;

  public ValueTask ConnectToServerAsync()
  {
    throw new NotImplementedException();
  }
}
