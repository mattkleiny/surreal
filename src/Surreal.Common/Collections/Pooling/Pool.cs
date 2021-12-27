using System.Collections;

namespace Surreal.Collections.Pooling;

/// <summary>A pool of objects of type, <see cref="T"/>.</summary>
public sealed class Pool<T>
{
	/// <summary>A shared pool for instances of type, <see cref="T"/>.</summary>
	public static Pool<T> Shared { get; } = new(Activator.CreateInstance<T>);

	private readonly Func<T> factory;
	private readonly BoundedQueue<T> instances;

	public Pool(Func<T> factory, int capacity = 0, int maxCapacity = 32)
	{
		this.factory = factory;
		instances = new BoundedQueue<T>(capacity, maxCapacity);
	}

	public T CreateOrRent()
	{
		if (!instances.TryDequeue(out var result))
		{
			result = factory();
		}

		if (result is IPoolAware aware)
		{
			aware.OnRent();
		}

		return result;
	}

	public void Return(T value)
	{
		if (value is IPoolAware aware)
		{
			aware.OnReturn();
		}
		else if (value is IList list)
		{
			list.Clear();
		}

		instances.TryEnqueue(value);
	}

	public void Clear()
	{
		instances.Clear();
	}
}
