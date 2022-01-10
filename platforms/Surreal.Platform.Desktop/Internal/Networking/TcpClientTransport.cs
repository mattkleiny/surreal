using System.Net;
using System.Net.Sockets;
using Surreal.Networking.Transports;

namespace Surreal.Internal.Networking;

internal sealed class TcpClientTransport : SocketTransport, IClientTransport
{
  private readonly TransportOptions options;

  public TcpClientTransport(TransportOptions options)
    : base(SocketType.Stream)
  {
    this.options = options;
  }

  public TransportType Type => TransportType.Reliability;

  public async ValueTask ConnectToServerAsync()
  {
    await Socket.ConnectAsync(IPAddress.Parse("127.0.0.1"), options.Port);
  }
}
