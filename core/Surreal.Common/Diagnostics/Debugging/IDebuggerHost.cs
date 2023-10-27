using Surreal.Reactive;

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

/// <summary>
/// An event that is being propagated across a <see cref="IDebuggerHost"/>.
/// </summary>
public sealed class DebuggerEvent
{
  /// <summary>
  /// Creates a new <see cref="DebuggerEvent"/> with the given kind and value.
  /// </summary>
  public static DebuggerEvent Create<T>(DebuggerEventKind<T> kind, T value)
  {
    return new DebuggerEvent
    {
      Kind = kind.Name,
      Payload = JsonSerializer.Serialize(value)
    };
  }

  /// <summary>
  /// The kind of event. Indicative of a <see cref="DebuggerEventKind{T}.Name"/>.
  /// </summary>
  public required string Kind { get; init; }

  /// <summary>
  /// The actual payload contents that are being sent for this particular event.
  /// </summary>
  public required string Payload { get; init; }
}

/// <summary>
/// A kind of <see cref="DebuggerEvent"/>, and it's associated data type.
/// </summary>
[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public sealed record DebuggerEventKind<T>(string Name);

/// <summary>
/// Helpers for working with <see cref="DebuggerEvent"/>s.
/// </summary>
public static class DebuggerEventExtensions
{
  /// <summary>
  /// Subscribes to a stream of <see cref="DebuggerEvent"/>s of the given kind.
  /// </summary>
  public static IDisposable Subscribe<T>(this IObservable<DebuggerEvent> observable, DebuggerEventKind<T> kind, Action<T> onNext)
  {
    return observable.Where(kind).Subscribe(onNext);
  }

  /// <summary>
  /// Filters a stream of <see cref="DebuggerEvent"/>s to only those of a specific kind.
  /// </summary>
  public static IObservable<T> Where<T>(this IObservable<DebuggerEvent> observable, DebuggerEventKind<T> kind)
  {
    return observable
      .Where(it => string.Equals(it.Kind, kind.Name, StringComparison.OrdinalIgnoreCase))
      .Select(it => JsonSerializer.Deserialize<T>(it.Payload)!);
  }
}
