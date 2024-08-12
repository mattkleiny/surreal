namespace Surreal.Collections.Concurrent;

/// <summary>
/// A concurrent version of the <see cref="RingBuffer{T}"/>.
/// </summary>
public sealed class ConcurrentRingBuffer<T>(int capacity) : IEnumerable<T>
  where T : notnull
{
  private readonly T[] _elements = new T[capacity];
  private int _count;
  private int _head;

  public int Count => _count;
  public int Capacity => _elements.Length;

  public T this[Index index] => _elements[index];

  public T First => _elements[_head];
  public T Last => _elements[Math.Max(_head - 1, 0)];

  public void Add(T element)
  {
    var index = Interlocked.Increment(ref _head);

    _elements[index] = element;

    if (_head >= Capacity)
    {
      _head = 0; // wrap around ring end
    }

    if (Count < Capacity)
    {
      Interlocked.Increment(ref _count);
    }
  }

  public IEnumerator<T> GetEnumerator()
  {
    throw new NotImplementedException();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}
