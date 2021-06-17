using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Surreal.IO;

namespace Surreal.Mathematics.Tensors {
  public abstract class Tensor<T> : IEnumerable<T>
      where T : unmanaged {
    protected Tensor(IBuffer<T> buffer) {
      Buffer = buffer;
    }

    public IBuffer<T> Buffer { get; }
    public int        Length => Buffer.Length;
    public int        Stride => Buffer.Stride;
    public Size       Size   => Buffer.Size;

    public void Clear()       => Fill(default);
    public void Fill(T value) => Buffer.Fill(value);

    public Enumerator             GetEnumerator() => new(this);
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();

    protected static char GetSubscript(int n) => (char) int.Parse(("U+208" + n).Substring(2), NumberStyles.HexNumber);

    public struct Enumerator : IEnumerator<T> {
      private readonly Tensor<T> tensor;
      private          int       index;

      public Enumerator(Tensor<T> tensor)
          : this() {
        this.tensor = tensor;
        Reset();
      }

      public T           Current    => tensor.Buffer.Span[index];
      object IEnumerator.Current    => Current;
      public bool        MoveNext() => ++index < tensor.Length;
      public void        Reset()    => index = -1;

      public void Dispose() {
      }
    }
  }
}