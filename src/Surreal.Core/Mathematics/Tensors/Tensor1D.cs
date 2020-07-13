using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Surreal.IO;

namespace Surreal.Mathematics.Tensors {
  [DebuggerDisplay("{ToDebuggerString()}")]
  public sealed class Tensor1D<T> : Tensor<T>, ITensor<T>
      where T : unmanaged {
    public Tensor1D(int length)
        : this(Buffers.Allocate<T>(length), length) {
    }

    public Tensor1D(IBuffer<T> buffer, int length)
        : base(buffer) {
      Check.That(length       > 0, "Length > 0");
      Check.That(buffer.Count >= length, "buffer.Count >= length");

      Length = length;
    }

    public int   Length { get; }
    public int   Rank   => 1;
    public int[] Shape  => new[] {Length};

    public T this[int index] {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get {
        CheckBounds(index);

        return Buffer.Span[index];
      }
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      set {
        CheckBounds(index);

        Buffer.Span[index] = value;
      }
    }

    T ITensor<T>.this[params int[] ranks] {
      get {
        Check.That(ranks.Length == 1, "ranks.Length == 1");

        var index = ranks[0];

        return this[index];
      }
      set {
        Check.That(ranks.Length == 1, "ranks.Length == 1");

        var index = ranks[0];

        this[index] = value;
      }
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
    private void CheckBounds(int index) {
      if (index < 0 || index >= Length) throw new IndexOutOfRangeException($"{index} is not in the range [0, {Length})");
    }

    private string ToDebuggerString() => $"⊗{GetSubscript(Rank)} (Length={Length}, Size={Size})";
  }
}