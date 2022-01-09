using System.Windows.Threading;
using Surreal.Threading;

namespace Surreal.Internal;

/// <summary>A <see cref="IDispatcher"/> that's coordinated by the WPF dispatcher</summary>
internal sealed class WindowsDispatcher : IDispatcher
{
  private readonly DispatcherAwaitable awaitable;

  public WindowsDispatcher(Dispatcher dispatcher)
  {
    awaitable = new DispatcherAwaitable(dispatcher);
  }

  public IDispatcher.IDispatcherAwaitable Yield()
  {
    return awaitable;
  }

  private sealed class DispatcherAwaitable : IDispatcher.IDispatcherAwaitable
  {
    private readonly DispatcherAwaiter awaiter;

    public DispatcherAwaitable(Dispatcher dispatcher)
    {
      awaiter = new DispatcherAwaiter(dispatcher);
    }

    public IDispatcher.IDispatchAwaiter GetAwaiter()
    {
      return awaiter;
    }

    private sealed class DispatcherAwaiter : IDispatcher.IDispatchAwaiter
    {
      private readonly Dispatcher dispatcher;

      public DispatcherAwaiter(Dispatcher dispatcher)
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
        dispatcher.BeginInvoke(continuation);
      }
    }
  }
}
