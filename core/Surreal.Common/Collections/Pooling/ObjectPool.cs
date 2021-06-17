using System;
using System.Collections;

namespace Surreal.Collections.Pooling {
  public sealed class ObjectPool<T>
      where T : class, new() {
    public static ObjectPool<T> Shared { get; } = new();

    private readonly BoundedQueue<T> queue;

    public ObjectPool(int capacity = 0, int maxCapacity = 32) {
      queue = new BoundedQueue<T>(capacity, maxCapacity);
    }

    public T CreateOrRent() {
      if (!queue.TryDequeue(out var result)) {
        result = new T();
      }

      if (result is IPoolAware aware) {
        aware.OnRent();
      }

      return result;
    }

    public void Return(T value) {
      if (value is IPoolAware aware) {
        aware.OnReturn();
      }
      else if (value is IList list) {
        list.Clear();
      }

      queue.TryEnqueue(value);
    }

    public void Clear() {
      queue.Clear();
    }

    public RentalScope Borrow() {
      return new(this, CreateOrRent());
    }

    public readonly struct RentalScope : IDisposable {
      private readonly ObjectPool<T> pool;
      private readonly T             value;

      public RentalScope(ObjectPool<T> pool, T value) {
        this.pool  = pool;
        this.value = value;
      }

      public void Deconstruct(out T value) {
        value = this.value;
      }

      public T Value => value;

      public void Dispose() {
        pool.Return(value);
      }

      public static implicit operator T(RentalScope scope) => scope.value;
    }
  }
}