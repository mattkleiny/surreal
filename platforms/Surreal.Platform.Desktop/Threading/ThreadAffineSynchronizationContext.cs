namespace Surreal.Threading;

/// <summary>
/// A <see cref="SynchronizationContext"/> that prefers to schedule work back onto the main thread.
/// </summary>
internal sealed class ThreadAffineSynchronizationContext(Thread mainThread) : SynchronizationContext
{
  private readonly Queue<Continuation> _continuations = new();
  private readonly Queue<Continuation> _buffer = new();

  public void Process()
  {
    if (Thread.CurrentThread != mainThread)
    {
      throw new InvalidOperationException("Cannot process continuations from a non-main thread.");
    }

    while (_continuations.TryDequeue(out var continuation))
    {
      _buffer.Enqueue(continuation);
    }

    while (_buffer.TryDequeue(out var continuation))
    {
      continuation.Execute();
    }
  }

  public override void Post(SendOrPostCallback callback, object? state)
  {
    _continuations.Enqueue(new Continuation(callback, state));
  }

  public override void Send(SendOrPostCallback callback, object? state)
  {
    if (Thread.CurrentThread == mainThread)
    {
      callback(state);
    }
    else
    {
      _continuations.Enqueue(new Continuation(callback, state));
    }
  }

  private readonly record struct Continuation(SendOrPostCallback Callback, object? State)
  {
    public void Execute()
    {
      Callback(State);
    }
  }
}
