﻿using System.Runtime.ExceptionServices;

namespace Surreal.Fibers.Promises;

/// <summary>
/// A <see cref="Promise{T}"/> that propagates an exception.
/// </summary>
internal sealed class ExceptionPromise<T>(Exception exception) : IPromise<T>
{
  private readonly ExceptionDispatchInfo _exceptionInfo = ExceptionDispatchInfo.Capture(exception);

  public short Version { get; set; }

  public FiberTaskStatus GetStatus(short version)
  {
    return FiberTaskStatus.Faulted;
  }

  public T GetResult(short version)
  {
    _exceptionInfo.Throw();
    return default;
  }

  public void OnCompleted(Action continuation, short version)
  {
    continuation();
  }

  public void Cancel(short version)
  {
    // no-op
  }
}