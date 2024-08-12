using Surreal.Collections.Bounded;

namespace Surreal.Collections.Pooling;

/// <summary>
/// A pool of objects of type, <see cref="T" />.
/// </summary>
public sealed class Pool<T>(Func<T> factory, int maxCapacity = 32)
{
  /// <summary>
  /// A shared pool for instances of type, <see cref="T" />.
  /// </summary>
  public static Pool<T> Shared { get; } = new(Activator.CreateInstance<T>, maxCapacity: 1000);

  private readonly BoundedConcurrentQueue<T> _instances = new(maxCapacity);

  public T CreateOrRent()
  {
    if (!_instances.TryDequeue(out var result))
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
      aware.OnReturn();
    else if (value is IList list)
      list.Clear();
    else if (value is MemoryStream stream)
      stream.Seek(0, SeekOrigin.Begin);
    else if (value is StringBuilder builder)
      builder.Clear();

    _instances.TryEnqueue(value);
  }
}
