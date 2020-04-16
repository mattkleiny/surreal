using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Surreal.Collections
{
  public sealed class ActionQueue
  {
    private readonly ConcurrentQueue<Action> scheduledTasks = new ConcurrentQueue<Action>();
    private readonly Queue<Action>           executionPool  = new Queue<Action>();

    public void Enqueue(Action task)
    {
      scheduledTasks.Enqueue(task);
    }

    public void Execute()
    {
      if (scheduledTasks.Count <= 0) return;

      lock (executionPool)
      {
        while (scheduledTasks.TryDequeue(out var task))
        {
          executionPool.Enqueue(task);
        }

        while (executionPool.TryDequeue(out var task))
        {
          task.Invoke();
        }
      }
    }
  }
}
