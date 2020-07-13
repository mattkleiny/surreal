using System.Diagnostics;

namespace Surreal.Graphics {
  [DebuggerDisplay("Viewport {X}, {Y}, {Width}, {Height}")]
  public readonly struct Viewport {
    public readonly int X;
    public readonly int Y;
    public readonly int Width;
    public readonly int Height;

    public Viewport(int width, int height)
        : this(0, 0, width, height) {
    }

    public Viewport(int x, int y, int width, int height) {
      X      = x;
      Y      = y;
      Width  = width;
      Height = height;
    }
  }
}