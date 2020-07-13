using System;
using System.Threading;
using System.Threading.Tasks;

namespace Surreal.Framework.Editing.IPC {
  internal interface ICommunicationChannel : IDisposable, IAsyncDisposable {
    Task<TMessage> ReceiveAsync<TMessage>(CancellationToken cancellationToken = default)
        where TMessage : unmanaged;

    Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : unmanaged;
  }
}