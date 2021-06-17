using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Surreal.Data;
using Surreal.Mathematics.Grids;

namespace Surreal.Mathematics.Tensors {
  [DebuggerDisplay("{ToDebuggerString()}")]
  public sealed class Tensor2D<T> : Tensor<T>, ITensor<T>, IGrid<T>
      where T : unmanaged {
    public Tensor2D(int width, int height)
        : this(Buffers.Allocate<T>(width * height), width, height) {
    }

    public Tensor2D(IBuffer<T> buffer, int width, int height)
        : base(buffer) {
      Debug.Assert(width > 0, "Width > 0");
      Debug.Assert(height > 0, "Height > 0");
      Debug.Assert(buffer.Length >= width * height, "buffer.Length >= width * height");

      Width  = width;
      Height = height;
    }

    public int Width  { get; }
    public int Height { get; }

    public int   Rank  => 2;
    public int[] Shape => new[] {Width, Height};

    public T this[int x, int y] {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get {
        CheckBounds(x, y);

        return Buffer.Span[x + y * Width];
      }
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      set {
        CheckBounds(x, y);

        Buffer.Span[x + y * Width] = value;
      }
    }

    T ITensor<T>.this[params int[] ranks] {
      get {
        Debug.Assert(ranks.Length == 2, "ranks.Length == 2");

        var x = ranks[0];
        var y = ranks[1];

        return this[x, y];
      }
      set {
        Debug.Assert(ranks.Length == 2, "ranks.Length == 2");

        var x = ranks[0];
        var y = ranks[1];

        this[x, y] = value;
      }
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
    private void CheckBounds(int x, int y) {
      if (x < 0 || x >= Width) throw new IndexOutOfRangeException($"{x} is not in the range [0, {Width})");
      if (y < 0 || y >= Height) throw new IndexOutOfRangeException($"{y} is not in the range [0, {Height})");
    }

    private string ToDebuggerString() => $"⊗{GetSubscript(Rank)} (Width={Width}, Height={Height}, Size={Size})";
  }
}