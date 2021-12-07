using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Surreal.Collections.Pooling;

namespace Surreal.Fibers.Promises;

internal interface IPromise<out T>
{
  short Version { get; }

  FiberTaskStatus GetStatus(short version);
  T?              GetResult(short version);

  void OnCompleted(Action continuation, short version);
  void Cancel(short version);
}

internal abstract class Promise<T> : IPoolAware, IPromise<T>
{
  private readonly Queue<Action> continuations = new();

  private volatile int             version;
  private volatile FiberTaskStatus status;

  public    short           Version => (short) version;
  protected FiberTaskStatus Status  => status;

  protected void SetStatus(FiberTaskStatus status)
  {
    if (status != FiberTaskStatus.Pending &&
        status != FiberTaskStatus.Canceled)
    {
      RunContinuations();
    }

    this.status = status;
  }

  public FiberTaskStatus GetStatus(short version)
  {
    ValidateVersion(version);

    return status;
  }

  public virtual T? GetResult(short version)
  {
    ValidateVersion(version);

    return default;
  }

  public virtual void OnCompleted(Action continuation, short version)
  {
    ValidateVersion(version);

    if (status == FiberTaskStatus.Pending)
    {
      continuations.Enqueue(continuation);
    }
    else
    {
      continuation.Invoke();
    }
  }

  public virtual void Cancel(short version)
  {
    if (this.version == version)
    {
      status = FiberTaskStatus.Canceled;
    }
  }

  private void RunContinuations()
  {
    while (continuations.TryDequeue(out var continuation))
    {
      try
      {
        continuation.Invoke();
      }
      catch (Exception exception)
      {
        Debug.Print("An error occurred whilst running continuations: {0}", exception);
      }
    }
  }

  public virtual void OnRent()
  {
    status = FiberTaskStatus.Pending;
  }

  public virtual void OnReturn()
  {
    Interlocked.Increment(ref version);

    continuations.Clear();
  }

  [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Global")]
  protected void ValidateVersion(short version)
  {
    if (this.version != version)
    {
      throw new InvalidOperationException("Mis-matched fiber version is being used!");
    }
  }
}