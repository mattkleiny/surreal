using System.Collections;

namespace Surreal.Mathematics.Grids {
  public sealed class BitGrid : IGrid<bool> {
    private readonly BitArray elements;

    public BitGrid(int width, int height, bool defaultValue = default) {
      Width  = width;
      Height = height;

      elements = new BitArray(width * height, defaultValue);
    }

    public BitGrid(BitGrid other) {
      Width  = other.Width;
      Height = other.Height;

      elements = new BitArray(other.elements);
    }

    public int Width  { get; }
    public int Height { get; }

    public bool this[int x, int y] {
      get => elements[x + y * Width];
      set => elements[x + y * Width] = value;
    }
  }
}