using System;
using System.Diagnostics;

namespace Surreal.Collections.Spans {
  [DebuggerDisplay("SpanStack {Count}/{Capacity}")]
  public ref struct SpanStack<T>
      where T : unmanaged {
    private readonly Span<T> storage;

    public SpanStack(Span<T> storage) {
      this.storage = storage;

      Count = 0;
    }

    public int Count    { get; private set; }
    public int Capacity => storage.Length;

    public T this[int index] {
      get => storage[index];
      set => storage[index] = value;
    }

    public void Push(T element) {
      if (Count >= Capacity) {
        throw new Exception("Cannot add any more elements, it will overflow the buffer!");
      }

      storage[Count++] = element;
    }

    public bool TryPush(T element) {
      if (Count < Capacity) {
        Push(element);
        return true;
      }

      return false;
    }

    public T Pop() {
      if (Count < 0) {
        throw new Exception("Can't pop any more elements, it will underflow the buffer"!);
      }

      return storage[Count-- - 1];
    }

    public bool TryPop(out T element) {
      if (Count > 0) {
        element = Pop();
        return true;
      }

      element = default;
      return false;
    }

    public void Clear() {
      Count = 0;
    }

    public Span<T> ToSpan() {
      return storage.Slice(0, Count);
    }
  }
}