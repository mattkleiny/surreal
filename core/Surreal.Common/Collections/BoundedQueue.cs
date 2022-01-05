using System.Diagnostics.CodeAnalysis;

namespace Surreal.Collections;

/// <summary>A <see cref="Queue{T}"/> with a fixed-sized upper bound.</summary>
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

  public bool TryPeek([MaybeNullWhen(false)] out T result)
  {
    return queue.TryPeek(out result);
  }

  public bool TryEnqueue(T value)
  {
    if (queue.Count < maxCapacity)
    {
      queue.Enqueue(value);
      return true;
    }

    return false;
  }

  public bool TryDequeue([MaybeNullWhen(false)] out T result)
  {
    return queue.TryDequeue(out result);
  }

  public void Clear()
  {
    queue.Clear();
  }

  public Queue<T>.Enumerator    GetEnumerator() => queue.GetEnumerator();
  IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();
  IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
}

/// <summary>A <see cref="Queue{T}"/> with a fixed-sized upper bound.</summary>
public sealed class BoundedConcurrentQueue<T>
{
  private readonly ConcurrentQueue<T> queue;
  private readonly int                maxCapacity;

  public BoundedConcurrentQueue(int maxCapacity = 32)
  {
    queue = new ConcurrentQueue<T>();

    this.maxCapacity = maxCapacity;
  }

  public int Count    => queue.Count;
  public int Capacity => maxCapacity;

  public bool TryPeek([MaybeNullWhen(false)] out T result)
  {
    return queue.TryPeek(out result);
  }

  public bool TryEnqueue(T value)
  {
    if (queue.Count < maxCapacity)
    {
      queue.Enqueue(value);
      return true;
    }

    return false;
  }

  public bool TryDequeue([MaybeNullWhen(false)] out T result)
  {
    return queue.TryDequeue(out result);
  }

  public void Clear()
  {
    queue.Clear();
  }
}
