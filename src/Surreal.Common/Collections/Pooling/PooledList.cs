using System.Collections;

namespace Surreal.Collections.Pooling;

/// <summary>A generically pooled <see cref="List{T}"/>.</summary>
public sealed class PooledList<T> : IEnumerable<T>, IDisposable, IPoolAware
{
	private static Pool<PooledList<T>> Pool => Pool<PooledList<T>>.Shared;

	private readonly List<T> list = new();

	public static PooledList<T> CreateOrRent()
	{
		return Pool.CreateOrRent();
	}

	public void Add(T element) => list.Add(element);
	public void Remove(T element) => list.Remove(element);
	public void Clear() => list.Clear();
	public void Dispose() => Pool.Return(this);

	public Enumerator GetEnumerator() => new(this);
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

	void IPoolAware.OnRent()
	{
	}

	void IPoolAware.OnReturn()
	{
		list.Clear();
	}

	public struct Enumerator : IEnumerator<T>
	{
		private readonly PooledList<T> pooledList;
		private int index;

		public Enumerator(PooledList<T> pooledList)
		{
			this.pooledList = pooledList;
			index = -1;
		}

		public T Current => pooledList.list[index];
		object IEnumerator.Current => Current!;

		public bool MoveNext() => ++index < pooledList.list.Count;
		public void Reset() => index = -1;

		public void Dispose()
		{
			// no-op
		}
	}
}
