namespace Surreal.Diagnostics.Debugging;

/// <summary>
/// A host for debugging and interprocess communication.
/// </summary>
public interface IDebuggerHost
{
  /// <summary>
  /// A stream of <see cref="DebuggerEvent"/>s from the opposite side of the host.
  /// </summary>
  IObservable<DebuggerEvent> Events { get; }

  /// <summary>
  /// Enqueues an event to be sent at a later date.
  /// </summary>
  void EnqueueEvent(DebuggerEvent @event);

  /// <summary>
  /// Flushes all enqueued events to the opposite end of the channel.
  /// </summary>
  Task FlushAsync(CancellationToken cancellationToken = default);
}
