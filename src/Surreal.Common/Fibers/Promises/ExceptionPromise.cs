using System.Runtime.ExceptionServices;

namespace Surreal.Fibers.Promises;

internal sealed class ExceptionPromise<T> : IPromise<T>
{
  private readonly ExceptionDispatchInfo exceptionInfo;

  public ExceptionPromise(Exception exception)
  {
    exceptionInfo = ExceptionDispatchInfo.Capture(exception);
  }

  public short Version { get; set; }

  public FiberTaskStatus GetStatus(short version)
  {
    return FiberTaskStatus.Faulted;
  }

  public T GetResult(short version)
  {
    exceptionInfo.Throw();
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