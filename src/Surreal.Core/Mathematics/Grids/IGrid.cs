namespace Surreal.Mathematics.Grids {
  public interface IGrid<T> {
    int Width  { get; }
    int Height { get; }

    T this[int x, int y] { get; set; }

    bool IsValid(int x, int y) {
      return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    void Fill(T value) {
      for (var y = 0; y < Height; y++)
      for (var x = 0; x < Width; x++) {
        this[x, y] = value;
      }
    }
  }
}