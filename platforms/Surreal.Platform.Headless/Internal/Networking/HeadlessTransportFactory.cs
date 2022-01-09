using Surreal.Networking.Transports;

namespace Surreal.Internal.Networking;

internal sealed class HeadlessTransportFactory : ITransportFactory
{
  public IServerTransport CreateServerTransport(TransportOptions options)
  {
    return new HeadlessTransport(options.Type);
  }

  public IClientTransport CreateClientTransport(TransportOptions options)
  {
    return new HeadlessTransport(options.Type);
  }
}
