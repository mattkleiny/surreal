using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Surreal.Collections
{
  public sealed class BoundedQueue<T> : IEnumerable<T>
  {
    private readonly Queue<T> queue;
    private readonly int      maxCapacity;

    public BoundedQueue(int capacity = 0, int maxCapacity = 32)
    {
      Debug.Assert(capacity >= 0, "capacity >= 0");
      Debug.Assert(maxCapacity >= capacity, "maxCapacity >= capacity");

      queue = new Queue<T>(capacity);

      this.maxCapacity = maxCapacity;
    }

    public int Count    => queue.Count;
    public int Capacity => maxCapacity;

    public bool TryEnqueue(T value)
    {
      if (queue.Count < maxCapacity)
      {
        queue.Enqueue(value);
        return true;
      }

      return false;
    }

    public bool TryDequeue(out T result)
    {
      if (queue.Count > 0)
      {
        result = queue.Dequeue();
        return true;
      }

      result = default!;
      return false;
    }

    public void Clear()
    {
      queue.Clear();
    }

    public Queue<T>.Enumerator    GetEnumerator() => queue.GetEnumerator();
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
  }
}