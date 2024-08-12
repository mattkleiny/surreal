namespace Surreal.Fibers;

/// <summary>
/// Allows awaiting <see cref="FiberTask"/>s.
/// </summary>
public readonly struct FiberTaskAwaiter(FiberTask task) : INotifyCompletion
{
  [UsedImplicitly]
  public bool IsCompleted => task.Status != FiberTaskStatus.Pending;

  [UsedImplicitly]
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

/// <summary>
/// Allows awaiting <see cref="FiberTask{T}"/>s.
/// </summary>
public readonly struct FiberTaskAwaiter<T>(FiberTask<T> task) : INotifyCompletion
{
  [UsedImplicitly]
  public bool IsCompleted => task.Status != FiberTaskStatus.Pending;

  [UsedImplicitly]
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

/// <summary>
/// An awaitable yield operation for <see cref="FiberTask"/>s.
/// </summary>
public readonly struct FiberYieldAwaitable
{
  [UsedImplicitly]
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
  public Awaiter GetAwaiter()
  {
    return new Awaiter();
  }

  /// <summary>
  /// A no-op awaiter; used to schedule a continuation for the next frame.
  /// </summary>
  public readonly struct Awaiter : INotifyCompletion
  {
    [UsedImplicitly]
    public bool IsCompleted => false;

    [UsedImplicitly]
    public void GetResult()
    {
      // no-op
    }

    public void OnCompleted(Action continuation)
    {
      FiberScheduler.Current.Schedule(continuation);
    }
  }
}