namespace Surreal.Grids;

/// <summary>A grid of elements in 2-space.</summary>
public interface IGrid<T>
{
  public int Width  { get; }
  public int Height { get; }

  public T? this[int x, int y] { get; set; }

  bool IsValid(int x, int y)
  {
    return x >= 0 && x < Width && y >= 0 && y < Height;
  }

  void Fill(T value)
  {
    for (var y = 0; y < Height; y++)
    for (var x = 0; x < Width; x++)
    {
      this[x, y] = value;
    }
  }
}

/// <summary>A <see cref="IGrid{T}"/> with direct field access to <see cref="T"/>.</summary>
public interface IDirectAccessGrid<T> : IGrid<T>
{
  new ref T? this[int x, int y] { get; }
}