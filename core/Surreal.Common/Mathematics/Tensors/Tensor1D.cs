using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Surreal.Data;

namespace Surreal.Mathematics.Tensors {
  [DebuggerDisplay("{ToDebuggerString()}")]
  public sealed class Tensor1D<T> : Tensor<T>, ITensor<T>
      where T : unmanaged {
    public Tensor1D(int length)
        : this(Buffers.Allocate<T>(length), length) {
    }

    public Tensor1D(IBuffer<T> buffer, int length)
        : base(buffer) {
      Debug.Assert(length > 0, "Length > 0");
      Debug.Assert(buffer.Length >= length, "buffer.Length >= length");

      Length = length;
    }

    public new int   Length { get; }
    public     int   Rank   => 1;
    public     int[] Shape  => new[] {Length};

    public T this[int index] {
      get {
        CheckBounds(index);

        return Buffer.Data[index];
      }
      set {
        CheckBounds(index);

        Buffer.Data[index] = value;
      }
    }

    T ITensor<T>.this[params int[] ranks] {
      get {
        Debug.Assert(ranks.Length == 1, "ranks.Length == 1");

        var index = ranks[0];

        return this[index];
      }
      set {
        Debug.Assert(ranks.Length == 1, "ranks.Length == 1");

        var index = ranks[0];

        this[index] = value;
      }
    }

    [Conditional("DEBUG")]
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
    private void CheckBounds(int index) {
      if (index < 0 || index >= Length) throw new IndexOutOfRangeException($"{index} is not in the range [0, {Length})");
    }

    private string ToDebuggerString() => $"⊗{GetSubscript(Rank).ToString()} (Length={Length.ToString()}, Size={Size.ToString()})";
  }
}