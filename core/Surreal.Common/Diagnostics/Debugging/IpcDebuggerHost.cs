using System.Runtime.Versioning;
using Surreal.Reactive;
using TinyIpc.Messaging;

namespace Surreal.Diagnostics.Debugging;

/// <summary>
/// A <see cref="IDebuggerHost"/> that uses IPC to communicate with the debugger.
/// <para/>
/// The primary means of communication is via <see cref="DebuggerEvent"/>s that
/// are forwarded to and fro from client to server.
/// <para/>
/// The connection is resilient to either the client or server disconnecting and
/// reconnecting at any time, though the events will not be replayed in this case.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class IpcDebuggerHost : IDisposable, IDebuggerHost
{
  private readonly Observable<DebuggerEvent> _events = new();
  private readonly Queue<DebuggerEvent> _eventsToSend = new();
  private readonly TinyMessageBus _messageBus;

  public IpcDebuggerHost(string channelName)
  {
    _messageBus = new TinyMessageBus(channelName);
    _messageBus.MessageReceived += OnMessageReceived;
  }

  /// <inheritdoc/>
  public IObservable<DebuggerEvent> Events => _events;

  /// <summary>
  /// Enqueues an event to be sent at a later date.
  /// </summary>
  public void EnqueueEvent(DebuggerEvent @event)
  {
  }

  /// <summary>
  /// Flushes all enqueued events to the opposite end of the channel.
  /// </summary>
  public async Task FlushAsync(CancellationToken cancellationToken = default)
  {
    while (!cancellationToken.IsCancellationRequested && _eventsToSend.TryDequeue(out var @event))
    {
      await _messageBus.PublishAsync(JsonSerializer.SerializeToUtf8Bytes(@event));
    }
  }

  /// <summary>
  /// Callback when a message is received from the opposite end of the channel.
  /// </summary>
  private void OnMessageReceived(object? sender, TinyMessageReceivedEventArgs e)
  {
    try
    {
      var bytes = (byte[])e.Message;
      var value = JsonSerializer.Deserialize<DebuggerEvent>(bytes);

      if (value != null)
      {
        _events.NotifyNext(value);
      }
    }
    catch (Exception exception)
    {
      _events.NotifyError(exception);
    }
  }

  public void Dispose()
  {
    _events.NotifyCompleted();
    _messageBus.Dispose();
  }
}
