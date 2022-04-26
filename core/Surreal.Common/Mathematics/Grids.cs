﻿using Surreal.Memory;

namespace Surreal.Mathematics;

/// <summary>A dense 2d grid of <see cref="T"/>.</summary>
public sealed class Grid<T> : IEnumerable<T>
{
  private readonly T[] elements;

  public Grid(int width, int height)
  {
    Debug.Assert(width > 0, "width > 0");
    Debug.Assert(height > 0, "height > 0");

    elements = new T[width * height];

    Width  = width;
    Height = height;
  }

  public Grid(int width, int height, T defaultValue)
    : this(width, height)
  {
    Fill(defaultValue);
  }

  public int Width  { get; }
  public int Height { get; }

  public SpanGrid<T> Span => new(elements, Width);

  public ref T this[int x, int y]
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

  public Enumerator GetEnumerator() => new(this);
  IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  /// <summary>Custom enumerator for <see cref="Grid{T}"/>s.</summary>
  public struct Enumerator : IEnumerator<T>
  {
    private readonly Grid<T> grid;
    private int index;

    public Enumerator(Grid<T> grid)
      : this()
    {
      this.grid = grid;
      Reset();
    }

    public T           Current => grid.elements[index]!;
    object IEnumerator.Current => Current!;
    public bool MoveNext() => ++index < grid.elements.Length;
    public void Reset() => index = -1;

    public void Dispose()
    {
    }
  }
}

/// <summary>A sparse 2d grid of <see cref="T"/>.</summary>
public sealed class SparseGrid<T> : IEnumerable<T>
{
  private readonly Dictionary<Point2, T> items = new();

  public T? this[int x, int y]
  {
    get => this[new Point2(x, y)];
    set => this[new Point2(x, y)] = value;
  }

  public T? this[Point2 position]
  {
    get
    {
      if (!items.TryGetValue(position, out var item))
      {
        return default;
      }

      return item;
    }
    set
    {
      if (value != null)
      {
        items[position] = value;
      }
      else
      {
        items.Remove(position);
      }
    }
  }

  public void Clear()
  {
    items.Clear();
  }

  public Dictionary<Point2, T>.ValueCollection.Enumerator GetEnumerator()
  {
    return items.Values.GetEnumerator();
  }

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return items.Values.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}
