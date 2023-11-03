namespace Surreal.Networking.Transports;

/// <summary>
/// Server-side <see cref="ITransport"/>.
/// </summary>
public interface IServerTransport : ITransport
{
  /// <summary>
  /// Attempts to start the server.
  /// </summary>
  ValueTask StartServerAsync(CancellationToken cancellationToken = default);
}
