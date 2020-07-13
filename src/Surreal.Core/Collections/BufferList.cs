using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Surreal.IO;

namespace Surreal.Collections {
  public sealed class BufferList<T> : IEnumerable<T>, IDisposable
      where T : unmanaged {
    private readonly IBuffer<T> buffer;

    public BufferList(IBuffer<T> buffer) {
      this.buffer = buffer;
    }

    public int     Count    { get; private set; }
    public int     Capacity => buffer.Count;
    public Span<T> Span     => buffer.Span;

    public void Add(T element) {
      Debug.Assert(Count < Capacity, "Count < Capacity");

      Span[Count++] = element;
    }

    public void Dispose() {
      if (buffer is IDisposable disposable) {
        disposable.Dispose();
      }
    }

    public Enumerator             GetEnumerator() => new Enumerator(this);
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<T> {
      private readonly BufferList<T> list;
      private          int           index;
      private          int           touched;

      public Enumerator(BufferList<T> list)
          : this() {
        this.list = list;
        Reset();
      }

      public T           Current => list.Span[index];
      object IEnumerator.Current => Current!;

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