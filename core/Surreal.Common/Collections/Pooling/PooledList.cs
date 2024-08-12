namespace Surreal.Collections.Pooling;

/// <summary>
/// A <see cref="List{T}" /> variant that is sourced from some <see cref="Pool{T}"/> data structure.
/// </summary>
public sealed class PooledList<T> : List<T>, IDisposable
{
  /// <summary>
  /// A shared pool for instances of type, <see cref="PooledList{T}" />.
  /// </summary>
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

  public void Dispose()
  {
    _pool.Return(this);
  }
}
