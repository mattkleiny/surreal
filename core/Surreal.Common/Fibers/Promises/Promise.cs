using Surreal.Collections.Pooling;
using Surreal.Diagnostics.Logging;

namespace Surreal.Fibers.Promises;

/// <summary>
/// A <see cref="FiberTask{T}"/>-compatible promise pattern for async/await support.
/// </summary>
internal interface IPromise<out T>
{
  short Version { get; }

  FiberTaskStatus GetStatus(short version);
  T? GetResult(short version);

  void OnCompleted(Action continuation, short version);
  void Cancel(short version);
}

/// <summary>
/// Base class for any <see cref="IPromise{T}"/> implementation.
/// </summary>
internal abstract class Promise<T> : IPoolAware, IPromise<T>
{
  private static readonly ILog Log = LogFactory.GetLog<Promise<T>>();

  private readonly Queue<Action> _continuations = new();

  private volatile int _version;
  private volatile FiberTaskStatus _status;

  public short Version => (short)_version;
  protected FiberTaskStatus Status => _status;

  protected void SetStatus(FiberTaskStatus status)
  {
    if (status != FiberTaskStatus.Pending &&
        status != FiberTaskStatus.Canceled)
    {
      RunContinuations();
    }

    _status = status;
  }

  public FiberTaskStatus GetStatus(short version)
  {
    ValidateVersion(version);

    return _status;
  }

  public virtual T? GetResult(short version)
  {
    ValidateVersion(version);

    return default;
  }

  public virtual void OnCompleted(Action continuation, short version)
  {
    ValidateVersion(version);

    if (_status == FiberTaskStatus.Pending)
    {
      _continuations.Enqueue(continuation);
    }
    else
    {
      continuation.Invoke();
    }
  }

  public virtual void Cancel(short version)
  {
    if (_version == version)
    {
      _status = FiberTaskStatus.Canceled;
    }
  }

  // ReSharper disable Unity.PerformanceAnalysis
  private void RunContinuations()
  {
    while (_continuations.TryDequeue(out var continuation))
    {
      try
      {
        continuation.Invoke();
      }
      catch (Exception exception)
      {
        Log.Error(exception, "An unhanded exception was thrown while running a continuation.");
      }
    }
  }

  public virtual void OnRent()
  {
    _status = FiberTaskStatus.Pending;
  }

  public virtual void OnReturn()
  {
    Interlocked.Increment(ref _version);

    _continuations.Clear();
  }

  [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Global")]
  protected void ValidateVersion(short version)
  {
    if (_version != version)
    {
      throw new InvalidOperationException("Mis-matched fiber version is being used!");
    }
  }
}