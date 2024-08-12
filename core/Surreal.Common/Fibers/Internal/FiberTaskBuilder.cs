using System.Runtime.ExceptionServices;
using Surreal.Fibers.Promises;

namespace Surreal.Fibers.Internal;

/// <summary>
/// Compiler-service builder for async <see cref="FiberTask"/> methods.
/// </summary>
public struct FiberTaskBuilder
{
  private FiberTaskBuilder<Void> _builder;

  [UsedImplicitly, DebuggerHidden]
  public static FiberTaskBuilder Create()
  {
    return new();
  }

  [UsedImplicitly, DebuggerHidden]
  public FiberTask Task
  {
    get
    {
      if (_builder.Promise != null)
      {
        return new FiberTask(_builder.Promise, _builder.Promise.Version);
      }

      if (_builder.Exception != null)
      {
        return FiberTask.FromException(_builder.Exception);
      }

      return FiberTask.CompletedTask;
    }
  }

  [UsedImplicitly, DebuggerHidden]
  public void Start<TStateMachine>(ref TStateMachine stateMachine)
    where TStateMachine : IAsyncStateMachine
  {
    _builder.Start(ref stateMachine);
  }

  [UsedImplicitly, DebuggerHidden]
  public void SetStateMachine(IAsyncStateMachine stateMachine)
  {
    _builder.SetStateMachine(stateMachine);
  }

  [UsedImplicitly, DebuggerHidden]
  public void SetException(Exception exception)
  {
    _builder.SetException(exception);
  }

  [UsedImplicitly, DebuggerHidden]
  public void SetResult()
  {
    _builder.SetResult(default);
  }

  [UsedImplicitly, DebuggerHidden]
  public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
    where TAwaiter : INotifyCompletion
    where TStateMachine : IAsyncStateMachine
  {
    _builder.AwaitOnCompleted(ref awaiter, ref stateMachine);
  }

  [UsedImplicitly, DebuggerHidden]
  public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
    where TAwaiter : ICriticalNotifyCompletion
    where TStateMachine : IAsyncStateMachine
  {
    _builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
  }

  [UsedImplicitly, DebuggerHidden]
  private object? ObjectIdForDebugger => _builder.Promise;
}

[UsedImplicitly]
public struct FiberTaskBuilder<T>
{
  private IFiberTaskPromise<T>? _promise;
  private Exception? _exception;
  private T? _result;

  internal IFiberTaskPromise<T>? Promise => _promise;
  internal Exception? Exception => _exception;

  [UsedImplicitly, DebuggerHidden]
  public static FiberTaskBuilder<T> Create()
  {
    return new();
  }

  [UsedImplicitly, DebuggerHidden]
  public FiberTask<T?> Task
  {
    get
    {
      if (_promise != null)
      {
        return new FiberTask<T?>(_promise, _promise.Version);
      }

      if (_exception != null)
      {
        return FiberTask.FromException<T?>(_exception);
      }

      return FiberTask.FromResult(_result);
    }
  }

  [UsedImplicitly, DebuggerHidden]
  public void Start<TStateMachine>(ref TStateMachine stateMachine)
    where TStateMachine : IAsyncStateMachine
  {
    stateMachine.MoveNext();
  }

  [UsedImplicitly, DebuggerHidden]
  public void SetStateMachine(IAsyncStateMachine stateMachine)
  {
  }

  [UsedImplicitly, DebuggerHidden]
  public void SetException(Exception exception)
  {
    if (_promise != null)
    {
      _promise.SetException(exception);
    }
    else
    {
      _exception = exception;

      // immediately propagate un-promised exceptions
      ExceptionDispatchInfo.Capture(exception).Throw();
    }
  }

  [UsedImplicitly, DebuggerHidden]
  public void SetResult(T result)
  {
    if (_promise != null)
    {
      _promise.SetResult(result);
    }
    else
    {
      _result = result;
    }
  }

  [UsedImplicitly, DebuggerHidden]
  public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
    where TAwaiter : INotifyCompletion
    where TStateMachine : IAsyncStateMachine
  {
    if (_promise == null)
    {
      FiberTaskPromise<T, TStateMachine>.Allocate(ref stateMachine, out _promise);
    }

    awaiter.OnCompleted(_promise.AdvanceCallback);
  }

  [UsedImplicitly, DebuggerHidden]
  public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
    where TAwaiter : ICriticalNotifyCompletion
    where TStateMachine : IAsyncStateMachine
  {
    if (_promise == null)
    {
      FiberTaskPromise<T, TStateMachine>.Allocate(ref stateMachine, out _promise);
    }

    awaiter.UnsafeOnCompleted(_promise.AdvanceCallback);
  }

  [UsedImplicitly, DebuggerHidden]
  private object? ObjectIdForDebugger => _promise;
}