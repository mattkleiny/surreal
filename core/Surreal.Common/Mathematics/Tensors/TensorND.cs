using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Surreal.Data;

namespace Surreal.Mathematics.Tensors {
  [DebuggerDisplay("{ToDebuggerString()}")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public sealed class TensorND<T> : Tensor<T>, ITensor<T>
      where T : unmanaged {
    private readonly int[] offsets;

    public TensorND(params int[] shape)
        : this(Buffers.Allocate<T>(shape.Aggregate((a, b) => a * b)), shape) {
    }

    public TensorND(IBuffer<T> buffer, params int[] shape)
        : base(buffer) {
      Debug.Assert(shape.Length > 0, "Length > 0");

      Shape   = shape;
      offsets = CalculateOffsets(Shape);
    }

    public int   Rank  => Shape.Length;
    public int[] Shape { get; }

    public T this[params int[] ranks] {
      get {
        CheckBounds(ranks);

        return Buffer.Data[CalculateOffset(ranks)];
      }
      set {
        CheckBounds(ranks);

        Buffer.Data[CalculateOffset(ranks)] = value;
      }
    }

    private static int[] CalculateOffsets(int[] shape) {
      var result = new int[shape.Length];

      // x + y * Rank1 + z * Rank1 * Rank2 + w * Rank1 * Rank2 * Rank3 ...
      result[0] = 1;

      for (var i = 1; i < shape.Length; i++) {
        var factor = 1;

        for (var j = i - 1; j >= 0; j--) {
          factor *= shape[j];
        }

        result[i] = factor;
      }

      return result;
    }

    private int CalculateOffset(IReadOnlyList<int> ranks) {
      var offset = 0;

      for (var i = 0; i < Shape.Length; i++) {
        offset += ranks[i] * offsets[i];
      }

      return offset;
    }

    [Conditional("DEBUG")]
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
    private void CheckBounds(int[] ranks) {
      if (ranks.Length != Shape.Length) {
        throw new IndexOutOfRangeException($"Ranks are not of the expected cardinality {Rank}");
      }

      for (var i = 0; i < Shape.Length; i++) {
        if (ranks[i] < 0 || ranks[i] >= Shape[i]) {
          throw new IndexOutOfRangeException($"Rank is not in the range [0, {Shape[i]})");
        }
      }
    }

    private string ToDebuggerString() => $"⊗{GetSubscript(Rank).ToString()} Ranks=<{string.Join(" ", Shape)}>, Size={Size.ToString()})";
  }
}