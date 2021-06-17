using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Surreal.Data;

namespace Surreal.Mathematics.Tensors {
  public interface ITensor : IEnumerable {
    int   Rank  { get; }
    int[] Shape { get; }
  }

  public interface ITensor<T> : ITensor, IEnumerable<T> {
    T this[params int[] ranks] { get; set; }
  }

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

      public T           Current    => tensor.Buffer.Data[index];
      object IEnumerator.Current    => Current;
      public bool        MoveNext() => ++index < tensor.Length;
      public void        Reset()    => index = -1;

      public void Dispose() {
      }
    }
  }
}