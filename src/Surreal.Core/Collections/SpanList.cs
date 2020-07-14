using System;
using System.Diagnostics;

namespace Surreal.Collections {
  [DebuggerDisplay("SpanList {Length}/{Capacity}")]
  public ref struct SpanList<T>
    where T : unmanaged {
    private readonly Span<T> span;

    public SpanList(Span<T> span) {
      this.span = span;

      Length = 0;
    }

    public int Length   { get; private set; }
    public int Capacity => span.Length;

    public T this[Index index] {
      get => span[index];
      set => span[index] = value;
    }

    public void Add(T element) {
      if (Length > Capacity) {
        throw new Exception("Cannot add any more elements, it will overflow the buffer!");
      }

      span[Length++] = element;
    }

    public void Clear() {
      Length = 0;
    }

    public ReadOnlySpan<T> ToSpan() => span[..Length];
    public T[] ToArray() => ToSpan().ToArray();
  }
}