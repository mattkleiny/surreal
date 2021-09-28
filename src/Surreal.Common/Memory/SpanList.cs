using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Surreal.Memory
{
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

    public T this[int index]
    {
      get => storage[index];
      set => storage[index] = value;
    }

    public void Add(T element)
    {
      if (Count > Capacity)
      {
        throw new Exception("Cannot add any more elements, it will overflow the buffer!");
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
  }
}
