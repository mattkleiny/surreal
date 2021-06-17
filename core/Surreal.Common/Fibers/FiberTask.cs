using System;
using System.Runtime.CompilerServices;
using Surreal.Fibers.Internal;
using Surreal.Fibers.Promises;

namespace Surreal.Fibers {
  public enum FiberTaskStatus {
    Pending,
    Succeeded,
    Canceled,
    Faulted,
  }

  [AsyncMethodBuilder(typeof(FiberTaskBuilder))]
  public readonly struct FiberTask : IDisposable {
    public static FiberTask CompletedTask => default;

    public static FiberTask    Create(Func<FiberTask> factory)       => factory();
    public static FiberTask<T> Create<T>(Func<FiberTask<T>> factory) => factory();

    public static FiberTask<T> FromResult<T>(T value)                => new(value);
    public static FiberTask    FromException(Exception exception)    => FromPromise(new ExceptionPromise<Unit>(exception));
    public static FiberTask<T> FromException<T>(Exception exception) => FromPromise(new ExceptionPromise<T>(exception));

    public static FiberYieldAwaitable Yield()   => new();
    public static WhenAllBuilder      WhenAll() => new(WhenAllPromise.Create());

    internal static FiberTask    FromPromise(IPromise<Unit> promise) => new(promise, promise.Version);
    internal static FiberTask<T> FromPromise<T>(IPromise<T> promise) => new(promise, promise.Version);

    public static FiberTask WhenAll(FiberTask a, FiberTask b)
      => WhenAll().AddTask(a).AddTask(b).Begin();

    public static FiberTask WhenAll(FiberTask a, FiberTask b, FiberTask c)
      => WhenAll().AddTask(a).AddTask(b).AddTask(c).Begin();

    public static FiberTask WhenAll(FiberTask a, FiberTask b, FiberTask c, FiberTask d)
      => WhenAll().AddTask(a).AddTask(b).AddTask(c).AddTask(d).Begin();

    public static FiberTask Delay(TimeSpan duration, bool useUnscaledTime = true)
      => FromPromise(DelayPromise.Create(duration, useUnscaledTime));

    internal readonly IPromise<Unit>? Promise;
    internal readonly short           Version;

    internal FiberTask(IPromise<Unit> promise, short version) {
      Promise = promise;
      Version = version;
    }

    public FiberTaskStatus Status {
      get {
        if (Promise != null) {
          return Promise.GetStatus(Version);
        }

        return FiberTaskStatus.Succeeded;
      }
    }

    public FiberTaskAwaiter GetAwaiter() {
      return new(this);
    }

    public void Forget() {
      // no-op
    }

    public void Cancel()  => Promise?.Cancel(Version);
    public void Dispose() => Cancel();

    public readonly struct WhenAllBuilder {
      private readonly WhenAllPromise promise;

      internal WhenAllBuilder(WhenAllPromise promise) {
        this.promise = promise;
      }

      public WhenAllBuilder AddTask(FiberTask task) {
        promise.AddTask(task);

        return this;
      }

      public FiberTask Begin() {
        promise.Advance();

        return FromPromise(promise);
      }
    }
  }

  [AsyncMethodBuilder(typeof(FiberTaskBuilder<>))]
  public readonly struct FiberTask<T> : IDisposable {
    internal readonly IPromise<T>? Promise;
    internal readonly short        Version;
    internal readonly T?           Result;

    internal FiberTask(T result) {
      Promise = default;
      Version = default;
      Result  = result;
    }

    internal FiberTask(IPromise<T> promise, short version) {
      Promise = promise;
      Version = version;
      Result  = default;
    }

    public FiberTaskStatus Status {
      get {
        if (Promise != null) {
          return Promise.GetStatus(Version);
        }

        return FiberTaskStatus.Succeeded;
      }
    }

    public FiberTaskAwaiter<T> GetAwaiter() {
      return new(this);
    }

    public void Cancel()  => Promise?.Cancel(Version);
    public void Dispose() => Cancel();
  }
}