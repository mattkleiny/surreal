using System.Collections;

namespace Surreal.Collections.Pooling
{
  public sealed class Pool<T>
      where T : class, new()
  {
    public static Pool<T> Shared { get; } = new();

    private readonly BoundedQueue<T> queue;

    public Pool(int capacity = 0, int maxCapacity = 32)
    {
      queue = new BoundedQueue<T>(capacity, maxCapacity);
    }

    public T CreateOrRent()
    {
      if (!queue.TryDequeue(out var result))
      {
        result = new T();
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

      queue.TryEnqueue(value);
    }

    public void Clear()
    {
      queue.Clear();
    }
  }
}
