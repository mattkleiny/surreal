namespace Surreal.Mathematics.Grids
{
  public interface IDirectAccessGrid<T> : IGrid<T>
  {
    new ref T this[int x, int y] { get; }
  }
}