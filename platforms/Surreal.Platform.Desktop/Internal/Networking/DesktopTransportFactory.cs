using Surreal.Networking.Transports;

namespace Surreal.Internal.Networking;

internal sealed class DesktopTransportFactory : ITransportFactory
{
  public IServerTransport CreateServerTransport(TransportOptions options)
  {
    return options.Type switch
    {
      TransportType.Throughput  => new UdpServerTransport(),
      TransportType.Reliability => new TcpServerTransport(),

      _ => throw new InvalidOperationException($"An unrecognized transport type was requested: {options.Type}"),
    };
  }

  public IClientTransport CreateClientTransport(TransportOptions options)
  {
    return options.Type switch
    {
      TransportType.Throughput  => new UdpClientTransport(),
      TransportType.Reliability => new TcpClientTransport(),

      _ => throw new InvalidOperationException($"An unrecognized transport type was requested: {options.Type}"),
    };
  }
}
