using Surreal.Networking;
using Surreal.Networking.Transports;

namespace Surreal.Internal.Networking;

internal sealed class DesktopNetworkFactory : INetworkFactory
{
  public IServerTransport CreateServerTransport(TransportOptions options)
  {
    return options.Type switch
    {
      TransportType.Throughput  => new UdpServerTransport(options),
      TransportType.Reliability => new TcpServerTransport(options),

      _ => throw new InvalidOperationException($"An unrecognized transport type was requested: {options.Type}"),
    };
  }

  public IClientTransport CreateClientTransport(TransportOptions options)
  {
    return options.Type switch
    {
      TransportType.Throughput  => new UdpClientTransport(options),
      TransportType.Reliability => new TcpClientTransport(options),

      _ => throw new InvalidOperationException($"An unrecognized transport type was requested: {options.Type}"),
    };
  }
}
