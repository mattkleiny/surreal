namespace Surreal.Memory;

/// <summary>
/// A <see cref="Span{T}" /> that is operated like a <see cref="List{T}" />.
/// </summary>
[DebuggerDisplay("SpanList {Count}/{Capacity}")]
public ref struct SpanList<T>(Span<T> storage)
{
  private readonly Span<T> _storage = storage;

  public int Count { get; private set; } = 0;
  public int Capacity => _storage.Length;

  public ref T this[int index] => ref _storage[index];
  public ref T this[Index index] => ref _storage[index];

  public SpanList<T> this[Range range]
  {
    get
    {
      var (offset, _) = range.GetOffsetAndLength(_storage.Length);

      return new SpanList<T>(_storage[range])
      {
        Count = Math.Max(Count - offset, 0)
      };
    }
  }

  public void Add(T element)
  {
    if (Count >= Capacity)
    {
      throw new InvalidOperationException("Cannot add any more elements, it will overflow the buffer!");
    }

    _storage[Count++] = element;
  }

  public void AddUnchecked(T element)
  {
    _storage[Count++] = element;
  }

  public void Clear()
  {
    Count = 0;
  }

  public Span<T> ToSpan()
  {
    return _storage[..Count];
  }

  public static implicit operator Span<T>(SpanList<T> list)
  {
    return list.ToSpan();
  }

  public static implicit operator ReadOnlySpan<T>(SpanList<T> list)
  {
    return list.ToSpan();
  }
}
