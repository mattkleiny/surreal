using System;
using System.Collections;
using System.Collections.Generic;
using Surreal.IO;

namespace Surreal.Collections {
  public sealed class BufferList<T> : IDisposable, IReadOnlyList<T>
      where T : unmanaged {
    private readonly IBuffer<T> buffer;

    public BufferList(IBuffer<T> buffer) {
      this.buffer = buffer;
    }

    public int     Count    { get; private set; }
    public int     Capacity => buffer.Length;
    public Span<T> Span     => buffer.Span;

    public T this[int index] => Span[index];
    public T this[Index index] => Span[index];

    public void Add(T element) {
      if (Count >= Capacity) {
        if (buffer is IResizableBuffer<T> resizable) {
          resizable.Resize(Count * 2);
        }
        else {
          throw new Exception("BufferList overflow!");
        }
      }

      Span[Count++] = element;
    }

    public void Dispose() {
      if (buffer is IDisposable disposable) {
        disposable.Dispose();
      }
    }

    public Enumerator             GetEnumerator() => new(this);
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<T> {
      private readonly BufferList<T> list;
      private          int           index;

      public Enumerator(BufferList<T> list)
          : this() {
        this.list = list;
        Reset();
      }

      public T           Current => list.Span[index];
      object IEnumerator.Current => Current;

      public bool MoveNext() {
        return index++ < list.Count;
      }

      public void Reset() {
        index = 0;
      }

      public void Dispose() {
      }
    }
  }
}