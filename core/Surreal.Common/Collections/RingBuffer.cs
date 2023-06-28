namespace Surreal.Collections;

/// <summary>
/// A lightweight circular/ring buffer with fixed capacity.
/// </summary>
public sealed class RingBuffer<T>(int capacity) : IEnumerable<T>
{
  private T[] _elements = new T[capacity];
  private int _writePos = 0;

  public int Count { get; private set; } = 0;
  public int Capacity => _elements.Length;

  public ref T this[Index index] => ref _elements[index];
  public ref T Last => ref _elements[Math.Max(_writePos - 1, 0)];

  public void Add(T element)
  {
    _elements[_writePos++] = element;

    if (_writePos >= Capacity)
    {
      _writePos = 0; // wrap around ring end
    }

    if (Count < Capacity)
    {
      Count++; // track occupied slots
    }
  }

  public void Clear()
  {
    for (var i = 0; i < _elements.Length; i++) _elements[i] = default!; // help the GC

    _writePos = 0;
    Count = 0;
  }

  public void Resize(int size)
  {
    if (size < _elements.Length)
    {
      _writePos = Math.Max(_writePos, size - 1);
    }

    Array.Resize(ref _elements, size);
  }

  public Enumerator GetEnumerator()
  {
    return new Enumerator(this);
  }

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  public struct Enumerator : IEnumerator<T>
  {
    private readonly RingBuffer<T> _buffer;
    private int _currentPos;
    private int _touched;

    public Enumerator(RingBuffer<T> buffer)
      : this()
    {
      _buffer = buffer;
      Reset();
    }

    public ref T Current => ref _buffer._elements[_currentPos];
    T IEnumerator<T>.Current => _buffer._elements[_currentPos];
    object IEnumerator.Current => Current!;

    public bool MoveNext()
    {
      // wrap around the start of the buffer, iterating backwards
      if (--_currentPos < 0)
      {
        _currentPos = _buffer.Capacity - 1;
      }

      return _touched++ < _buffer.Count;
    }

    public void Reset()
    {
      _touched = 0;
      _currentPos = _buffer._writePos;
    }

    public void Dispose()
    {
      // no-op
    }
  }
}

/// <summary>
/// Extension methods for <see cref="RingBuffer{T}"/>.
/// </summary>
public static class RingBufferExtensions
{
  public static float FastSum(this RingBuffer<float> samples)
  {
    float total = 0;

    foreach (var sample in samples) total += sample;

    return total;
  }

  public static TimeSpan FastSum(this RingBuffer<TimeSpan> samples)
  {
    var total = TimeSpan.Zero;

    foreach (var sample in samples) total += sample;

    return total;
  }

  public static float FastAverage(this RingBuffer<float> samples)
  {
    return samples.FastSum() / samples.Count;
  }

  public static TimeSpan FastAverage(this RingBuffer<TimeSpan> samples)
  {
    var averageTicks = FastSum(samples).Ticks / samples.Count;

    return TimeSpan.FromTicks(averageTicks);
  }

  public static TimeSpan FastMin(this RingBuffer<TimeSpan> samples)
  {
    var result = TimeSpan.MaxValue;

    foreach (var sample in samples)
    {
      if (sample < result)
      {
        result = sample;
      }
    }

    return result;
  }

  public static TimeSpan FastMax(this RingBuffer<TimeSpan> samples)
  {
    var result = TimeSpan.MinValue;

    foreach (var sample in samples)
    {
      if (sample > result)
      {
        result = sample;
      }
    }

    return result;
  }
}
