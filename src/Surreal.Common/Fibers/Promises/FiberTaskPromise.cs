using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Surreal.Collections.Pooling;

namespace Surreal.Fibers.Promises;

internal interface IFiberTaskPromise<T> : IPromise<T>
{
  Action AdvanceCallback { get; }

  void SetException(Exception exception);
  void SetResult(T result);
}

internal sealed class FiberTaskPromise<T, TStateMachine> : Promise<T>, IFiberTaskPromise<T>
  where TStateMachine : IAsyncStateMachine
{
  private static readonly Pool<FiberTaskPromise<T, TStateMachine>> Pool = Pool<FiberTaskPromise<T, TStateMachine>>.Shared;

  public static void Allocate(ref TStateMachine stateMachine, out IFiberTaskPromise<T> promise)
  {
    var result = Pool.CreateOrRent();

    promise             = result;
    result.stateMachine = stateMachine;
  }

  private T?             result;
  private object?        error;
  private TStateMachine? stateMachine;
  private bool           isObserved;

  public Action AdvanceCallback { get; }
  public Action ReturnCallback  { get; }

  [DebuggerHidden] private void Advance() => stateMachine?.MoveNext();
  [DebuggerHidden] private void Return()  => Pool.Return(this);

  public FiberTaskPromise()
  {
    AdvanceCallback = Advance;
    ReturnCallback  = Return;
  }

  [DebuggerHidden]
  public override T? GetResult(short version)
  {
    ValidateVersion(version);

    isObserved = true;

    if (error != null)
    {
      if (error is Exception exception)
      {
        throw exception;
      }

      if (error is ExceptionDispatchInfo dispatchInfo)
      {
        dispatchInfo.Throw();
      }

      throw new InvalidOperationException("An unrecognized exception type was held!");
    }

    return result;
  }

  [DebuggerHidden]
  public void SetResult(T result)
  {
    this.result = result;

    SetStatus(FiberTaskStatus.Succeeded);
    FiberScheduler.Schedule(ReturnCallback);
  }

  [DebuggerHidden]
  public void SetException(Exception exception)
  {
    if (exception is OperationCanceledException)
    {
      error = exception;
    }
    else
    {
      error = ExceptionDispatchInfo.Capture(exception);
    }

    SetStatus(FiberTaskStatus.Faulted);
    FiberScheduler.Schedule(ReturnCallback);
  }

  [DebuggerHidden]
  public override void OnReturn()
  {
    base.OnReturn();

    if (!isObserved && error is Exception exception)
    {
      // TODO: log exception
    }

    result       = default;
    error        = default;
    stateMachine = default;
    isObserved   = default;
  }
}