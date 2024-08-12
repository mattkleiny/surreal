using Surreal.Collections.Pooling;

namespace Surreal.Fibers.Promises;

/// <summary>
/// A <see cref="Promise{T}"/> that awaits any given sub-tasks.
/// </summary>
internal sealed class WhenAnyPromise : Promise<Void>
{
  private static readonly Pool<WhenAnyPromise> Pool = new(() => new());

  private readonly Action _continueCallback;
  private readonly Action _returnCallback;

  private bool _isStarted;
  private int _completedCount;

  public static WhenAnyPromise Create()
  {
    return Pool.CreateOrRent();
  }

  private WhenAnyPromise()
  {
    _continueCallback = OnContinue;
    _returnCallback = () => Pool.Return(this);
  }

  public void AddTask(FiberTask task)
  {
    task.GetAwaiter().OnCompleted(_continueCallback);
  }

  private void OnContinue()
  {
    _completedCount += 1;

    if (Status == FiberTaskStatus.Canceled)
    {
      FiberScheduler.Current.Schedule(_returnCallback);
    }

    if (_isStarted)
    {
      CheckForCompletion();
    }
  }

  private void CheckForCompletion()
  {
    if (_completedCount > 0)
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
    _completedCount = 0;
  }
}