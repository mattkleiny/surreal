namespace Surreal.Memory;

/// <summary>
/// A <see cref="Span{T}" /> that is operated like a <see cref="Stack{T}" />.
/// </summary>
[DebuggerDisplay("SpanStack {Count}/{Capacity}")]
public ref struct SpanStack<T>(Span<T> storage)
{
  private readonly Span<T> _storage = storage;

  public int Count { get; private set; } = 0;
  public int Capacity => _storage.Length;

  public ref T this[Index index] => ref _storage[index];

  public SpanStack<T> this[Range range]
  {
    get
    {
      var (offset, _) = range.GetOffsetAndLength(_storage.Length);

      return new SpanStack<T>(_storage[range])
      {
        Count = Math.Max(Count - offset, 0)
      };
    }
  }

  public void Push(T element)
  {
    if (Count >= Capacity)
    {
      throw new InvalidOperationException("Cannot add any more elements, it will overflow the buffer!");
    }

    _storage[Count++] = element;
  }

  public bool TryPush(T element)
  {
    if (Count < Capacity)
    {
      Push(element);
      return true;
    }

    return false;
  }

  public T Pop()
  {
    if (Count < 0)
    {
      throw new InvalidOperationException("Can't pop any more elements, it will underflow the buffer");
    }

    return _storage[Count-- - 1];
  }

  public bool TryPop(out T element)
  {
    if (Count > 0)
    {
      element = Pop();
      return true;
    }

    element = default!;
    return false;
  }

  public void Clear()
  {
    Count = 0;
  }

  public Span<T> ToSpan()
  {
    return _storage[..Count];
  }

  public static implicit operator Span<T>(SpanStack<T> stack)
  {
    return stack.ToSpan();
  }

  public static implicit operator ReadOnlySpan<T>(SpanStack<T> stack)
  {
    return stack.ToSpan();
  }
}
