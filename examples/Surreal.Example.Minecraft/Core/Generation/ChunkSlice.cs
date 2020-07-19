using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Surreal.Mathematics.Linear;

namespace Minecraft.Core.Generation {
  [DebuggerDisplay("Slice offset {offset} - {dimensions}")]
  public sealed class ChunkSlice : IChunkView {
    private readonly Vector3I offset;
    private readonly Volume   dimensions;

    public ChunkSlice(Chunk chunk, Vector3I offset, Volume dimensions) {
      this.offset     = offset;
      this.dimensions = dimensions;

      Chunk = chunk;
    }

    public Chunk Chunk { get; }

    public int Width  => dimensions.Width;
    public int Height => dimensions.Height;
    public int Depth  => dimensions.Depth;

    public Block this[int x, int y, int z] {
      get {
        CheckBounds(x, y, z);
        return Chunk[x + offset.X, y + offset.Y, z + offset.Z];
      }
      set {
        CheckBounds(x, y, z);
        Chunk[x + offset.X, y + offset.Y, z + offset.Z] = value;
      }
    }

    [Conditional("DEBUG")]
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
    private void CheckBounds(int x, int y, int z) {
      if (x < 0 || x >= Width) throw new IndexOutOfRangeException($"x is not in the range [0, {Width})");
      if (y < 0 || y >= Height) throw new IndexOutOfRangeException($"y is not in the range [0, {Height})");
      if (z < 0 || z >= Depth) throw new IndexOutOfRangeException($"z is not in the range [0, {Depth})");
    }
  }
}