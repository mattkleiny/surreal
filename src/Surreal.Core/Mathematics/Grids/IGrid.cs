using System.Diagnostics;

namespace Surreal.Mathematics.Grids {
  public interface IGrid<T> {
    int Width  { get; }
    int Height { get; }

    T this[int x, int y] { get; set; }

    T Sample(int x, int y, GridSamplingMode mode = GridSamplingMode.Assert) {
      switch (mode) {
        case GridSamplingMode.Assert: {
          Debug.Assert(IsValid(x, y), "Contains(x,y)");
          break;
        }
        case GridSamplingMode.Clamp: {
          if (x < 0) x          = 0;
          if (y < 0) y          = 0;
          if (x > Width - 1) x  = Width - 1;
          if (y > Height - 1) y = Height - 1;
          break;
        }
        case GridSamplingMode.Wrap: {
          if (x < 0) x          = Width - 1;
          if (y < 0) y          = Height - 1;
          if (x > Width - 1) x  = 0;
          if (y > Height - 1) y = 0;
          break;
        }
      }

      return this[x, y];
    }

    bool IsValid(int x, int y) {
      return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    T TryGet(int x, int y, T defaultValue = default) {
      return IsValid(x, y) ? this[x, y] : defaultValue;
    }

    void Fill(T value) {
      for (var y = 0; y < Height; y++)
      for (var x = 0; x < Width; x++) {
        this[x, y] = value;
      }
    }
  }
}