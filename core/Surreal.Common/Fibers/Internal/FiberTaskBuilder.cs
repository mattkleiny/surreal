using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using JetBrains.Annotations;
using Surreal.Fibers.Promises;

namespace Surreal.Fibers.Internal {
  internal struct FiberTaskBuilder {
    private FiberTaskBuilder<Unit> builder;

    [UsedImplicitly, DebuggerHidden]
    public static FiberTaskBuilder Create() {
      return new();
    }

    [UsedImplicitly, DebuggerHidden]
    public FiberTask Task {
      get {
        if (builder.Promise != null) {
          return new FiberTask(builder.Promise, builder.Promise.Version);
        }

        if (builder.Exception != null) {
          return FiberTask.FromException(builder.Exception);
        }

        return FiberTask.CompletedTask;
      }
    }

    [UsedImplicitly, DebuggerHidden]
    public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine {
      builder.Start(ref stateMachine);
    }

    [UsedImplicitly, DebuggerHidden]
    public void SetStateMachine(IAsyncStateMachine stateMachine) {
      builder.SetStateMachine(stateMachine);
    }

    [UsedImplicitly, DebuggerHidden]
    public void SetException(Exception exception) {
      builder.SetException(exception);
    }

    [UsedImplicitly, DebuggerHidden]
    public void SetResult() {
      builder.SetResult(Unit.Default);
    }

    [UsedImplicitly, DebuggerHidden]
    public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine {
      builder.AwaitOnCompleted(ref awaiter, ref stateMachine);
    }

    [UsedImplicitly, DebuggerHidden]
    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine {
      builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
    }

    [UsedImplicitly, DebuggerHidden]
    private object? ObjectIdForDebugger => builder.Promise;
  }

  [UsedImplicitly]
  public struct FiberTaskBuilder<T> {
    private IFiberTaskPromise<T>? promise;
    private Exception?            exception;
    private T?                    result;

    internal IFiberTaskPromise<T>? Promise   => promise;
    internal Exception?            Exception => exception;

    [UsedImplicitly, DebuggerHidden]
    public static FiberTaskBuilder<T> Create() {
      return new();
    }

    [UsedImplicitly, DebuggerHidden]
    public FiberTask<T?> Task {
      get {
        if (promise != null) {
          return new FiberTask<T?>(promise, promise.Version);
        }

        if (exception != null) {
          return FiberTask.FromException<T?>(exception);
        }

        return FiberTask.FromResult(result);
      }
    }

    [UsedImplicitly, DebuggerHidden]
    public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine {
      stateMachine.MoveNext();
    }

    [UsedImplicitly, DebuggerHidden]
    public void SetStateMachine(IAsyncStateMachine stateMachine) {
    }

    [UsedImplicitly, DebuggerHidden]
    public void SetException(Exception exception) {
      if (promise != null) {
        promise.SetException(exception);
      }
      else {
        this.exception = exception;

        // immediately propagate un-promised exceptions
        ExceptionDispatchInfo.Capture(exception).Throw();
      }
    }

    [UsedImplicitly, DebuggerHidden]
    public void SetResult(T result) {
      if (promise != null) {
        promise.SetResult(result);
      }
      else {
        this.result = result;
      }
    }

    [UsedImplicitly, DebuggerHidden]
    public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine {
      if (promise == null) {
        FiberTaskPromise<T, TStateMachine>.Allocate(ref stateMachine, out promise);
      }

      awaiter.OnCompleted(promise.AdvanceCallback);
    }

    [UsedImplicitly, DebuggerHidden]
    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine {
      if (promise == null) {
        FiberTaskPromise<T, TStateMachine>.Allocate(ref stateMachine, out promise);
      }

      awaiter.UnsafeOnCompleted(promise.AdvanceCallback);
    }

    [UsedImplicitly, DebuggerHidden]
    private object? ObjectIdForDebugger => promise;
  }
}