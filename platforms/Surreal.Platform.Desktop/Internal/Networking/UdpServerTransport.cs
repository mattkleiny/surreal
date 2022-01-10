using System.Net.Sockets;
using Surreal.Networking.Transports;

namespace Surreal.Internal.Networking;

internal sealed class UdpServerTransport : SocketTransport, IServerTransport
{
  public UdpServerTransport(TransportOptions options)
    : base(SocketType.Dgram)
  {
  }

  public TransportType Type => TransportType.Throughput;

  public ValueTask StartServerAsync()
  {
    throw new NotImplementedException();
  }
}
