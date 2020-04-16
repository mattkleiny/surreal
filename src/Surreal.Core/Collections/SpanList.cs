using System;
using System.Diagnostics;

namespace Surreal.Collections
{
  [DebuggerDisplay("SpanList {Count}/{Capacity}")]
  public ref struct SpanList<T>
    where T : unmanaged
  {
    private readonly Span<T> span;

    public SpanList(Span<T> span)
    {
      this.span = span;

      Count = 0;
    }

    public int Count    { get; private set; }
    public int Capacity => span.Length;

    public T this[int index]
    {
      get => span[index];
      set => span[index] = value;
    }

    public void Add(T element)
    {
      if (Count > Capacity)
      {
        throw new Exception("Cannot add any more elements, it will overflow the buffer!");
      }

      span[Count++] = element;
    }

    public void Clear()
    {
      Count = 0;
    }

    public ReadOnlySpan<T> ToSpan()  => span[..Count];
    public T[]             ToArray() => ToSpan().ToArray();
  }
}