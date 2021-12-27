using System.Collections;
using Surreal.Mathematics;

namespace Surreal.Collections.Grids;

/// <summary>A sparsely packed <see cref="IGrid{T}"/> of <see cref="T"/>.</summary>
public sealed class SparseGrid<T> : IGrid<T>, IEnumerable<T>
	where T : class
{
	private readonly Dictionary<Vector2I, T> cells = new();
	private readonly Func<T, Vector2I> locator;

	public SparseGrid(Func<T, Vector2I> locator)
	{
		this.locator = locator;
	}

	public int Count => cells.Count;
	public int Bottom => cells.Values.Min(_ => locator(_).Y);
	public int Left => cells.Values.Min(_ => locator(_).X);
	public int Top => cells.Values.Max(_ => locator(_).Y) + 1;
	public int Right => cells.Values.Max(_ => locator(_).X) + 1;
	public int Width => Right - Left;
	public int Height => Top - Bottom;

	public T? this[int x, int y]
	{
		get => this[new Vector2I(x, y)];
		set => this[new Vector2I(x, y)] = value;
	}

	public T? this[Vector2I position]
	{
		get
		{
			if (cells.TryGetValue(position, out var cell))
			{
				return cell;
			}

			return default;
		}
		set
		{
			if (value != null)
			{
				cells[position] = value;
			}
			else
			{
				cells.Remove(position);
			}
		}
	}

	bool IGrid<T>.IsValid(int x, int y) => true;

	public Dictionary<Vector2I, T>.ValueCollection.Enumerator GetEnumerator()
	{
		return cells.Values.GetEnumerator();
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
