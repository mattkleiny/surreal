namespace Surreal.Collections;

/// <summary>Permits an object to respond to pool callbacks.</summary>
public interface IPoolAware
{
  void OnRent();
  void OnReturn();
}

/// <summary>A pool of objects of type, <see cref="T"/>.</summary>
public sealed class Pool<T>
{
  /// <summary>A shared pool for instances of type, <see cref="T"/>.</summary>
  public static Pool<T> Shared { get; } = new(Activator.CreateInstance<T>);

  private readonly Func<T> factory;
  private readonly BoundedConcurrentQueue<T> instances;

  public Pool(Func<T> factory, int maxCapacity = 32)
  {
    this.factory = factory;
    instances = new BoundedConcurrentQueue<T>(maxCapacity);
  }

  public T CreateOrRent()
  {
    if (!instances.TryDequeue(out var result))
    {
      result = factory();
    }

    switch (result)
    {
      case IPoolAware aware:
        {
          aware.OnRent();
          break;
        }
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
    else if (value is MemoryStream stream)
    {
      stream.Seek(0, SeekOrigin.Begin);
    }
    else if (value is StringBuilder builder)
    {
      builder.Clear();
    }

    instances.TryEnqueue(value);
  }

  public void Clear()
  {
    instances.Clear();
  }
}

/// <summary>A generically pooled <see cref="List{T}"/>.</summary>
public sealed class PooledList<T> : IEnumerable<T>, IDisposable, IPoolAware
{
  private static Pool<PooledList<T>> Pool => Pool<PooledList<T>>.Shared;

  public static PooledList<T> CreateOrRent() => Pool.CreateOrRent();

  private readonly List<T> list = new(capacity: 0);

  public void Add(T element)
  {
    list.Add(element);
  }

  public void Remove(T element)
  {
    list.Remove(element);
  }

  public void Clear()
  {
    list.Clear();
  }

  public void Dispose()
  {
    Pool.Return(this);
  }

  public List<T>.Enumerator GetEnumerator()
  {
    return list.GetEnumerator();
  }

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  void IPoolAware.OnRent()
  {
    // no-op
  }

  void IPoolAware.OnReturn()
  {
    list.Clear();
  }
}
