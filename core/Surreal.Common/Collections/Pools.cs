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
}

/// <summary>
/// A <see cref="List{T}" /> variant that is sourced from some <see cref="Surreal.Collections.Pool{T}"/> data structure.
/// </summary>
public sealed class PooledList<T> : List<T>, IDisposable
{
  /// <summary>
  /// A shared pool for instances of type, <see cref="PooledList{T}" />.
  /// </summary>
  [SuppressMessage("ReSharper", "RedundantSuppressNullableWarningExpression")]
  private static Pool<PooledList<T>> Shared { get; } = new(() => new PooledList<T>(Shared!), maxCapacity: 1000);

  /// <summary>
  /// Creates or rents a <see cref="PooledList{T}" /> from a shared pool.
  /// </summary>
  public static PooledList<T> CreateOrRent()
  {
    return Shared.CreateOrRent();
  }

  private readonly Pool<PooledList<T>> _pool;

  private PooledList(Pool<PooledList<T>> pool, int capacity = 0)
    : base(capacity)
  {
    _pool = pool;
  }

  /// <summary>
  /// Slices the <see cref="PooledList{T}" /> into a <see cref="Slice{T}" />.
  /// </summary>
  public Slice<T> this[Range range]
  {
    get
    {
      var (offset, length) = range.GetOffsetAndLength(Count);

      return new Slice<T>(this, offset, length);
    }
  }

  public void Dispose()
  {
    _pool.Return(this);
  }
}
