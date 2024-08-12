namespace Surreal.Collections.Bounded;

/// <summary>
/// A <see cref="Queue{T}" /> with a fixed-sized upper bound.
/// </summary>
public sealed class BoundedQueue<T> : IEnumerable<T>
{
  private readonly Queue<T> _queue;

  public BoundedQueue(int capacity = 0, int maxCapacity = 32)
  {
    Debug.Assert(capacity >= 0);
    Debug.Assert(maxCapacity >= capacity);

    _queue = new Queue<T>(capacity);

    Capacity = maxCapacity;
  }

  public int Count => _queue.Count;
  public int Capacity { get; }

  public bool TryPeek([MaybeNullWhen(false)] out T result)
  {
    return _queue.TryPeek(out result);
  }

  public bool TryEnqueue(T value)
  {
    if (_queue.Count < Capacity)
    {
      _queue.Enqueue(value);
      return true;
    }

    return false;
  }

  public bool TryDequeue([MaybeNullWhen(false)] out T result)
  {
    return _queue.TryDequeue(out result);
  }

  public void Clear()
  {
    _queue.Clear();
  }

  public Queue<T>.Enumerator GetEnumerator()
  {
    return _queue.GetEnumerator();
  }

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}
