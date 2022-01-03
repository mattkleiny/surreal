using System.Runtime.CompilerServices;

namespace Surreal.Fibers;

/// <summary>Allows awaiting <see cref="FiberTask"/> results.</summary>
public readonly struct FiberTaskAwaiter : INotifyCompletion
{
  private readonly FiberTask task;

  public FiberTaskAwaiter(FiberTask task)
  {
    this.task = task;
  }

  public bool IsCompleted => task.Status != FiberTaskStatus.Pending;

  public void GetResult()
  {
    task.Promise?.GetResult(task.Version);
  }

  public void OnCompleted(Action continuation)
  {
    if (task.Promise != null)
    {
      task.Promise.OnCompleted(continuation, task.Version);
    }
    else
    {
      continuation();
    }
  }
}

/// <summary>Allows awaiting <see cref="FiberTask{T}"/> results.</summary>
public readonly struct FiberTaskAwaiter<T> : INotifyCompletion
{
  private readonly FiberTask<T> task;

  public FiberTaskAwaiter(FiberTask<T> task)
  {
    this.task = task;
  }

  public bool IsCompleted => task.Status != FiberTaskStatus.Pending;

  public T? GetResult()
  {
    if (task.Promise != null)
    {
      return task.Promise.GetResult(task.Version);
    }

    return task.Result;
  }

  public void OnCompleted(Action continuation)
  {
    if (task.Promise != null)
    {
      task.Promise.OnCompleted(continuation, task.Version);
    }
    else
    {
      continuation();
    }
  }
}
