namespace Surreal.Memory;

/// <summary>A <see cref="Span{T}"/> that is operated like a <see cref="List{T}"/>.</summary>
[DebuggerDisplay("SpanList {Count}/{Capacity}")]
public ref struct SpanList<T>
  where T : unmanaged
{
  private readonly Span<T> storage;

  public SpanList(Span<T> storage)
  {
    this.storage = storage;

    Count = 0;
  }

  public int Count    { get; private set; }
  public int Capacity => storage.Length;

  public ref T this[Index index] => ref storage[index];

  public Span<T> this[Range range] => storage[range];

  public void Add(T element)
  {
    if (Count > Capacity)
    {
      throw new InvalidOperationException("Cannot add any more elements, it will overflow the buffer!");
    }

    storage[Count++] = element;
  }

  public void Clear()
  {
    Count = 0;
  }

  public Span<T> ToSpan()
  {
    return storage[..Count];
  }

  public static implicit operator Span<T>(SpanList<T> list) => list.ToSpan();
}
