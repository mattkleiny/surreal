namespace Surreal.Collections;

/// <summary>
/// Permits an object to respond to pool callbacks.
/// </summary>
public interface IPoolAware
{
  void OnRent();
  void OnReturn();
}

/// <summary>
/// A pool of objects of type, <see cref="T" />.
/// </summary>
public sealed class Pool<T>
{
  private readonly Func<T> _factory;
  private readonly BoundedConcurrentQueue<T> _instances;

  public Pool(Func<T> factory, int maxCapacity = 32)
  {
    _factory = factory;
    _instances = new BoundedConcurrentQueue<T>(maxCapacity);
  }

  /// <summary>
  /// A shared pool for instances of type, <see cref="T" />.
  /// </summary>
  public static Pool<T> Shared { get; } = new(Activator.CreateInstance<T>);

  public T CreateOrRent()
  {
    if (!_instances.TryDequeue(out var result))
    {
      result = _factory();
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

    _instances.TryEnqueue(value);
  }

  public void Clear()
  {
    _instances.Clear();
  }
}
