using System.Net;
using System.Net.Sockets;
using Surreal.Networking.Transports;

namespace Surreal.Internal.Networking;

internal sealed class TcpServerTransport : SocketTransport, IServerTransport
{
  private readonly TransportOptions options;

  public TcpServerTransport(TransportOptions options)
    : base(SocketType.Stream)
  {
    this.options = options;
  }

  public TransportType Type => TransportType.Reliability;

  public ValueTask StartServerAsync()
  {
    Socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), options.Port));
    Socket.Listen();

    return ValueTask.CompletedTask;
  }
}
