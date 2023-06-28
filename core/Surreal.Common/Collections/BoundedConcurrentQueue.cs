namespace Surreal.Collections;

/// <summary>
/// A <see cref="ConcurrentQueue{T}" /> with a fixed-sized upper bound.
/// </summary>
public sealed class BoundedConcurrentQueue<T>(int maxCapacity = 32)
{
  private readonly ConcurrentQueue<T> _queue = new();

  public int Count => _queue.Count;
  public int Capacity { get; } = maxCapacity;

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
}
