using Surreal.Networking.Transports;

namespace Surreal.Internal.Networking;

internal sealed class HeadlessTransport : IHeadlessTransport, IClientTransport, IServerTransport
{
  public HeadlessTransport(TransportType type)
  {
    Type = type;
  }

  public TransportType Type { get; }

  public ValueTask ConnectToServerAsync()
  {
    return ValueTask.CompletedTask;
  }

  public ValueTask StartServerAsync()
  {
    return ValueTask.CompletedTask;
  }

  public ValueTask SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
  {
    return ValueTask.CompletedTask;
  }

  public ValueTask ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
  {
    return ValueTask.CompletedTask;
  }

  public void Dispose()
  {
    // no-op
  }

  public ValueTask DisposeAsync()
  {
    return ValueTask.CompletedTask;
  }
}
