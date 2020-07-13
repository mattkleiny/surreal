using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Surreal.Collections {
  public interface IPoolAware {
    void Reset();
  }

  [DebuggerDisplay("Pool {Occupied}/{Capacity}")]
  public sealed class ObjectPool<T> {
    private readonly ConcurrentBag<T> pool = new ConcurrentBag<T>();

    public ObjectPool(int capacity) {
      Check.That(capacity > 0, "capacity > 0");

      Capacity = capacity;
    }

    public int Capacity { get; }
    public int Occupied => pool.Count;

    public bool TryRent(out T item) {
      return pool.TryTake(out item);
    }

    public T RentOrCreate(Func<T> factory) {
      return TryRent(out var result) ? result : factory();
    }

    public void Return(T value) {
      if (Occupied < Capacity) {
        if (value is IPoolAware aware) {
          aware.Reset();
        }

        pool.Add(value);
      }
    }
  }
}