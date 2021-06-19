using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Surreal.Grids
{
  public sealed class DenseGrid<T> : IEnumerable<T>, IGrid<T>
  {
    private readonly T?[] elements;

    public DenseGrid(int width, int height, T defaultValue)
    {
      Debug.Assert(width > 0, "width > 0");
      Debug.Assert(height > 0, "height > 0");

      Width  = width;
      Height = height;

      elements = new T[width * height];

      Fill(defaultValue);
    }

    public int      Width  { get; }
    public int      Height { get; }
    public Span<T?> Span   => elements;

    T? IGrid<T>.this[int x, int y]
    {
      get => this[x, y];
      set => this[x, y] = value;
    }

    public ref T? this[int x, int y]
    {
      get
      {
        Debug.Assert(x >= 0 && x < Width, "x >= 0 && x < Width");
        Debug.Assert(y >= 0 && y < Height, "y >= 0 && y < Height");

        return ref elements[x + y * Width];
      }
    }

    public void Fill(T value)
    {
      for (var i = 0; i < elements.Length; i++)
      {
        elements[i] = value;
      }
    }

    public Enumerator             GetEnumerator() => new(this);
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<T>
    {
      private readonly DenseGrid<T> grid;
      private          int          index;

      public Enumerator(DenseGrid<T> grid)
          : this()
      {
        this.grid = grid;
        Reset();
      }

      public T           Current    => grid.elements[index]!;
      object IEnumerator.Current    => Current!;
      public bool        MoveNext() => ++index < grid.elements.Length;
      public void        Reset()    => index = -1;

      public void Dispose()
      {
      }
    }
  }
}