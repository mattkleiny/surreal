namespace Surreal.Mathematics.Grids
{
  public interface IGrid<T>
  {
    int Width  { get; }
    int Height { get; }

    T this[int x, int y] { get; set; }
  }
}
