using Surreal.Networking.Transports;

namespace Surreal.Networking;

/// <summary>
/// A factory for network related services.
/// </summary>
public interface INetworkFactory
{
  /// <summary>
  /// Creates a <see cref="IServerTransport"/> for the given <paramref name="options"/>.
  /// </summary>
  IServerTransport CreateServerTransport(TransportOptions options);

  /// <summary>
  /// Creates a <see cref="IClientTransport"/> for the given <paramref name="options"/>.
  /// </summary>
  IClientTransport CreateClientTransport(TransportOptions options);
}
