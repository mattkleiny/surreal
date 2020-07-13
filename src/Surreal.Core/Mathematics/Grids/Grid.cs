using System.Collections;
using System.Collections.Generic;

namespace Surreal.Mathematics.Grids {
  public sealed class Grid<T> : IEnumerable<T>, IDirectAccessGrid<T> {
    private readonly T[] elements;

    public Grid(int width, int height, T defaultValue = default) {
      Check.That(width  > 0, "width > 0");
      Check.That(height > 0, "height > 0");

      Width  = width;
      Height = height;

      elements = new T[width * height];

      Fill(defaultValue);
    }

    public int Width  { get; }
    public int Height { get; }

    public ref T this[int x, int y] {
      get {
        Check.That(x >= 0, "x >= 0");
        Check.That(y >= 0, "y >= 0");
        Check.That(x < Width, "x < Width");
        Check.That(y < Height, "y < Height");

        return ref elements[x + y * Width];
      }
    }

    T IGrid<T>.this[int x, int y] {
      get => this[x, y];
      set => this[x, y] = value;
    }

    public void Fill(T value) {
      for (var i = 0; i < elements.Length; i++) {
        elements[i] = value;
      }
    }

    public Enumerator             GetEnumerator() => new Enumerator(this);
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<T> {
      private readonly Grid<T> grid;
      private          int     index;

      public Enumerator(Grid<T> grid)
          : this() {
        this.grid = grid;
        Reset();
      }

      public T           Current    => grid.elements[index];
      object IEnumerator.Current    => Current!;
      public bool        MoveNext() => ++index < grid.elements.Length;
      public void        Reset()    => index = -1;

      public void Dispose() {
      }
    }
  }
}