namespace Surreal.Networking.Transports;

/// <summary>
/// Client-side <see cref="ITransport"/>.
/// </summary>
public interface IClientTransport : ITransport
{
  /// <summary>
  /// Attempts to connect to the server.
  /// </summary>
  /// <param name="cancellationToken"></param>
  ValueTask ConnectToServerAsync(CancellationToken cancellationToken = default);
}
