using System.Collections;
using System.Runtime.CompilerServices;

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
      get {
        Check.That(x >= 0 && x < Width, "x >= 0 && x < Width");
        Check.That(y >= 0 && y < Height, "y >= 0 && y < Height");
        
        return elements[x + y * Width];
      }
      set {
        Check.That(x >= 0 && x < Width, "x >= 0 && x < Width");
        Check.That(y >= 0 && y < Height, "y >= 0 && y < Height");
        
        elements[x + y * Width] = value;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fill(bool value) {
      elements.SetAll(value);
    }
  }
}