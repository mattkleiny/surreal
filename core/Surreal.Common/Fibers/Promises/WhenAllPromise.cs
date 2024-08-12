using Surreal.Collections.Pooling;

namespace Surreal.Fibers.Promises;

/// <summary>
/// A <see cref="Promise{T}"/> that awaits all given sub-tasks.
/// </summary>
internal sealed class WhenAllPromise : Promise<Void>
{
  private static readonly Pool<WhenAllPromise> Pool = new(() => new());

  private readonly Action _continueCallback;
  private readonly Action _cancelCallback;
  private readonly Action _returnCallback;

  private bool _isStarted;
  private int _totalCount;
  private int _completedCount;
  private CancellationTokenRegistration _cancellationRegistration;

  public static WhenAllPromise Create(CancellationToken cancellationToken)
  {
    var promise = Pool.CreateOrRent();

    promise._cancellationRegistration = cancellationToken.Register(promise._cancelCallback);

    return promise;
  }

  private WhenAllPromise()
  {
    _continueCallback = OnContinue;
    _cancelCallback = OnCancel;

    _returnCallback = () => Pool.Return(this);
  }

  public void AddTask(FiberTask task)
  {
    _totalCount += 1;

    task.GetAwaiter().OnCompleted(_continueCallback);
  }

  private void OnContinue()
  {
    _completedCount += 1;

    if (Status == FiberTaskStatus.Canceled)
    {
      FiberScheduler.Current.Schedule(_returnCallback);
    }
    else if (_isStarted)
    {
      CheckForCompletion();
    }
  }

  private void OnCancel()
  {
    SetStatus(FiberTaskStatus.Canceled);
    FiberScheduler.Current.Schedule(_returnCallback);
  }

  private void CheckForCompletion()
  {
    if (_completedCount >= _totalCount)
    {
      SetStatus(FiberTaskStatus.Succeeded);

      FiberScheduler.Current.Schedule(_returnCallback);
    }
  }

  public void Advance()
  {
    _isStarted = true;

    CheckForCompletion();
  }

  public override void OnReturn()
  {
    base.OnReturn();

    _isStarted = false;
    _totalCount = 0;
    _completedCount = 0;
    _cancellationRegistration.Dispose();
  }
}