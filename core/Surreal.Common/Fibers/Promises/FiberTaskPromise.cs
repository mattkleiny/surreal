using System.Runtime.ExceptionServices;
using Surreal.Collections.Pooling;
using Surreal.Diagnostics.Logging;

namespace Surreal.Fibers.Promises;

/// <summary>
/// A promise that can be used to await a <see cref="FiberTask"/>.
/// </summary>
internal interface IFiberTaskPromise<T> : IPromise<T>
{
  Action AdvanceCallback { get; }

  void SetException(Exception exception);
  void SetResult(T result);
}

/// <summary>
/// A <see cref="Promise{T}"/> over another <see cref="FiberTask"/>.
/// </summary>
internal sealed class FiberTaskPromise<T, TStateMachine> : Promise<T>, IFiberTaskPromise<T>
  where TStateMachine : IAsyncStateMachine
{
  private static readonly ILog Log = LogFactory.GetLog<FiberTaskPromise<T, TStateMachine>>();

  private static readonly Pool<FiberTaskPromise<T, TStateMachine>> Pool = new(() => new());

  public static void Allocate(ref TStateMachine stateMachine, out IFiberTaskPromise<T> promise)
  {
    var result = Pool.CreateOrRent();

    promise = result;
    result._stateMachine = stateMachine;
  }

  private T? _result;
  private object? _error;
  private TStateMachine? _stateMachine;
  private bool _isObserved;

  public Action AdvanceCallback { get; }
  public Action ReturnCallback { get; }

  [DebuggerHidden]
  private void Advance()
  {
    if (Status != FiberTaskStatus.Canceled)
    {
      _stateMachine?.MoveNext();
    }
    else
    {
      Return();
    }
  }

  [DebuggerHidden]
  private void Return()
  {
    Pool.Return(this);
  }

  private FiberTaskPromise()
  {
    AdvanceCallback = Advance;
    ReturnCallback = Return;
  }

  [DebuggerHidden]
  public override T? GetResult(short version)
  {
    ValidateVersion(version);

    _isObserved = true;

    if (_error != null)
    {
      if (_error is Exception exception)
      {
        throw exception;
      }

      if (_error is ExceptionDispatchInfo dispatchInfo)
      {
        dispatchInfo.Throw();
      }

      throw new InvalidOperationException("An unrecognized exception type was held!");
    }

    return _result;
  }

  [DebuggerHidden]
  public void SetResult(T result)
  {
    _result = result;

    SetStatus(FiberTaskStatus.Succeeded);
    FiberScheduler.Current.Schedule(ReturnCallback);
  }

  [DebuggerHidden]
  public void SetException(Exception exception)
  {
    if (exception is OperationCanceledException)
    {
      _error = exception;
    }
    else
    {
      _error = ExceptionDispatchInfo.Capture(exception);
    }

    SetStatus(FiberTaskStatus.Faulted);
    FiberScheduler.Current.Schedule(ReturnCallback);
  }

  [DebuggerHidden]
  public override void OnReturn()
  {
    base.OnReturn();

    if (!_isObserved && _error is Exception exception)
    {
      Log.Error(exception, "An unobserved exception was thrown");
    }

    _result = default;
    _error = default;
    _stateMachine = default;
    _isObserved = default;
  }
}