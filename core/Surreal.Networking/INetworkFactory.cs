using Surreal.Networking.Transports;

namespace Surreal.Networking;

/// <summary>A factory for network related services.</summary>
public interface INetworkFactory
{
  IServerTransport CreateServerTransport(TransportOptions options);
  IClientTransport CreateClientTransport(TransportOptions options);
}
