using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace Surreal.Framework.Editing.IPC
{
  internal sealed class PipeCommunicationChannel : ICommunicationChannel
  {
    private readonly PipeStream stream;

    public static PipeCommunicationChannel CreateClient(string pipeName, string serverName = ".")
      => new PipeCommunicationChannel(new NamedPipeClientStream(
        serverName: serverName,
        pipeName: pipeName,
        direction: PipeDirection.InOut,
        options: PipeOptions.Asynchronous
      ));

    public static PipeCommunicationChannel CreateServer(string pipeName)
      => new PipeCommunicationChannel(new NamedPipeServerStream(
        pipeName: pipeName,
        direction: PipeDirection.InOut,
        maxNumberOfServerInstances: NamedPipeServerStream.MaxAllowedServerInstances,
        transmissionMode: PipeTransmissionMode.Message,
        options: PipeOptions.Asynchronous
      ));

    private PipeCommunicationChannel(PipeStream stream)
    {
      this.stream = stream;
    }

    public Task<TMessage> ReceiveAsync<TMessage>(CancellationToken cancellationToken = default)
      where TMessage : unmanaged
    {
      throw new NotImplementedException();
    }

    public Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
      where TMessage : unmanaged
    {
      throw new NotImplementedException();
    }

    public void      Dispose()      => stream.Dispose();
    public ValueTask DisposeAsync() => stream.DisposeAsync();
  }
}