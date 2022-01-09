using System.Runtime.CompilerServices;

namespace Surreal;

// TODO: rethink this

/// <summary>Dispatcher for game operations. Allows coordinating game loop with game hosting. </summary>
public interface IGameDispatcher
{
  void Schedule(Action continuation);

  /// <summary>Yields to the next frame of the dispatcher.</summary>
  GameDispatcherYieldAwaitable Yield() => new(this);

  /// <summary>Awaits until the next frame of the game dispatcher.</summary>
  public readonly struct GameDispatcherYieldAwaitable
  {
    private readonly IGameDispatcher dispatcher;

    public GameDispatcherYieldAwaitable(IGameDispatcher dispatcher)
    {
      this.dispatcher = dispatcher;
    }

    public GameDispatcherYieldAwaiter GetAwaiter()
    {
      return new GameDispatcherYieldAwaiter(dispatcher);
    }

    /// <summary>An awaiter for the game dispatcher.</summary>
    public readonly struct GameDispatcherYieldAwaiter : INotifyCompletion
    {
      private readonly IGameDispatcher dispatcher;

      public GameDispatcherYieldAwaiter(IGameDispatcher dispatcher)
      {
        this.dispatcher = dispatcher;
      }

      public bool IsCompleted => dispatcher != null;

      public void GetResult()
      {
        // no-op
      }

      public void OnCompleted(Action continuation)
      {
        dispatcher?.Schedule(continuation);
      }
    }
  }
}
