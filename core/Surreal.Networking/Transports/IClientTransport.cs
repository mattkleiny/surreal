namespace Surreal.Networking.Transports;

/// <summary>
/// Client-side <see cref="ITransport"/>.
/// </summary>
public interface IClientTransport : ITransport
{
  ValueTask ConnectToServerAsync();
}