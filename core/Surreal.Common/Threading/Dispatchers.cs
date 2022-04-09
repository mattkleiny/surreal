using System.Runtime.CompilerServices;

namespace Surreal.Threading;

/// <summary>A dispatcher for operations. Allows coordinating logic with hosting. </summary>
public interface IDispatcher
{
  /// <summary>Yields to the next step of the dispatcher.</summary>
  IDispatcherAwaitable Yield();

  /// <summary>An awaitable for the dispatcher.</summary>
  public interface IDispatcherAwaitable
  {
    IDispatchAwaiter GetAwaiter();
  }

  /// <summary>An awaiter for the dispatcher.</summary>
  public interface IDispatchAwaiter : INotifyCompletion
  {
    bool IsCompleted { get; }
    void GetResult();
  }
}

/// <summary>A <see cref="IDispatcher"/> that continues execution immediately.</summary>
public sealed class ImmediateDispatcher : IDispatcher
{
  private readonly ImmediateAwaitable awaitable = new();

  public IDispatcher.IDispatcherAwaitable Yield()
  {
    return awaitable;
  }

  private sealed class ImmediateAwaitable : IDispatcher.IDispatcherAwaitable
  {
    private readonly DispatcherAwaiter awaiter = new();

    public IDispatcher.IDispatchAwaiter GetAwaiter()
    {
      return awaiter;
    }

    private sealed class DispatcherAwaiter : IDispatcher.IDispatchAwaiter
    {
      public bool IsCompleted => true;

      public void GetResult()
      {
        // no-op
      }

      public void OnCompleted(Action continuation)
      {
        // no-op
      }
    }
  }
}

/// <summary>A <see cref="IDispatcher"/> that defers back to a central executor</summary>
public sealed class DeferredDispatcher : IDispatcher
{
  private readonly ConcurrentQueue<Action> continuations = new();
  private readonly DeferredAwaitable awaitable;

  public DeferredDispatcher()
  {
    awaitable = new DeferredAwaitable(this);
  }

  /// <summary>Runs all deferred continuations immediately.</summary>
  public void RunContinuations()
  {
    while (continuations.TryDequeue(out var continuation))
    {
      continuation.Invoke();
    }
  }

  public IDispatcher.IDispatcherAwaitable Yield()
  {
    return awaitable;
  }

  private sealed class DeferredAwaitable : IDispatcher.IDispatcherAwaitable
  {
    private readonly DispatcherAwaiter awaiter;

    public DeferredAwaitable(DeferredDispatcher dispatcher)
    {
      awaiter = new DispatcherAwaiter(dispatcher);
    }

    public IDispatcher.IDispatchAwaiter GetAwaiter()
    {
      return awaiter;
    }

    private sealed class DispatcherAwaiter : IDispatcher.IDispatchAwaiter
    {
      private readonly DeferredDispatcher dispatcher;

      public DispatcherAwaiter(DeferredDispatcher dispatcher)
      {
        this.dispatcher = dispatcher;
      }

      public bool IsCompleted => false;

      public void GetResult()
      {
        // no-op
      }

      public void OnCompleted(Action continuation)
      {
        dispatcher.continuations.Enqueue(continuation);
      }
    }
  }
}
