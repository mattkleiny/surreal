namespace Surreal.Networking.Transports;

/// <summary>
/// Server-side <see cref="ITransport"/>.
/// </summary>
public interface IServerTransport : ITransport
{
  ValueTask StartServerAsync();
}
