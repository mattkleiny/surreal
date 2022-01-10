using System.Net.Sockets;

namespace Surreal.Internal.Networking;

/// <summary><see cref="Socket"/>-based transport operations.</summary>
internal abstract class SocketTransport : IDisposable, IAsyncDisposable
{
  protected SocketTransport(SocketType socketType, ProtocolType protocolType = ProtocolType.IP)
    : this(new Socket(socketType, protocolType))
  {
  }

  protected SocketTransport(Socket socket)
  {
    Socket = socket;

    socket.Blocking = false;
  }

  public Socket Socket { get; }

  public async ValueTask SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
  {
    await Socket.SendAsync(buffer, SocketFlags.None, cancellationToken);
  }

  public async ValueTask ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
  {
    await Socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken);
  }

  public void Dispose()
  {
    Socket.Dispose();
  }

  public ValueTask DisposeAsync()
  {
    Dispose();

    return ValueTask.CompletedTask;
  }
}
