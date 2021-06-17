using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Surreal.Data;

namespace Surreal.Mathematics.Tensors {
  [DebuggerDisplay("{ToDebuggerString()}")]
  public sealed class Tensor3D<T> : Tensor<T>, ITensor<T>
      where T : unmanaged {
    public Tensor3D(int width, int height, int depth)
        : this(Buffers.Allocate<T>(width * height * depth), width, height, depth) {
    }

    public Tensor3D(IBuffer<T> buffer, int width, int height, int depth)
        : base(buffer) {
      Debug.Assert(width > 0, "Width > 0");
      Debug.Assert(height > 0, "Height > 0");
      Debug.Assert(depth > 0, "Depth > 0");
      Debug.Assert(buffer.Length >= width * height * depth, "buffer.Length >= width * height * depth");

      Width  = width;
      Height = height;
      Depth  = depth;
    }

    public int Width  { get; }
    public int Height { get; }
    public int Depth  { get; }

    public int   Rank  => 3;
    public int[] Shape => new[] {Width, Height, Depth};

    public T this[int x, int y, int z] {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get {
        CheckBounds(x, y, z);

        return Buffer.Span[x + y * Width + z * Width * Height];
      }
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      set {
        CheckBounds(x, y, z);

        Buffer.Span[x + y * Width + z * Width * Height] = value;
      }
    }

    T ITensor<T>.this[params int[] ranks] {
      get {
        Debug.Assert(ranks.Length == 3, "ranks.Length == 3");

        var x = ranks[0];
        var y = ranks[1];
        var z = ranks[2];

        return this[x, y, z];
      }
      set {
        Debug.Assert(ranks.Length == 3, "ranks.Length == 3");

        var x = ranks[0];
        var y = ranks[1];
        var z = ranks[2];

        this[x, y, z] = value;
      }
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
    private void CheckBounds(int x, int y, int z) {
      if (x < 0 || x >= Width) throw new IndexOutOfRangeException($"{x} is not in the range [0, {Width})");
      if (y < 0 || y >= Height) throw new IndexOutOfRangeException($"{y} is not in the range [0, {Height})");
      if (z < 0 || z >= Depth) throw new IndexOutOfRangeException($"{z} is not in the range [0, {Depth})");
    }

    private string ToDebuggerString() => $"⊗{GetSubscript(Rank)} (Width={Width}, Height={Height}, Depth={Depth}, Size={Size})";
  }
}