using System.Collections;
using System.Diagnostics;

namespace Surreal.Collections;

/// <summary>A <see cref="Stack{T}"/> with a fixed-sized upper bound.</summary>
public sealed class BoundedStack<T> : IEnumerable<T>
{
	private readonly Stack<T> queue;
	private readonly int maxCapacity;

	public BoundedStack(int capacity = 0, int maxCapacity = 32)
	{
		Debug.Assert(capacity >= 0, "capacity >= 0");
		Debug.Assert(maxCapacity >= capacity, "maxCapacity >= capacity");

		queue = new Stack<T>(capacity);

		this.maxCapacity = maxCapacity;
	}

	public int Count => queue.Count;
	public int Capacity => maxCapacity;

	public bool TryPush(T value)
	{
		if (queue.Count < maxCapacity)
		{
			queue.Push(value);
			return true;
		}

		return false;
	}

	public bool TryPop(out T result)
	{
		if (queue.Count > 0)
		{
			result = queue.Pop();
			return true;
		}

		result = default!;
		return false;
	}

	public void Clear()
	{
		queue.Clear();
	}

	public Stack<T>.Enumerator GetEnumerator() => queue.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
}
