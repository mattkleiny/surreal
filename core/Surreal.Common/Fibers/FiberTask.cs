using Surreal.Fibers.Internal;
using Surreal.Fibers.Promises;

namespace Surreal.Fibers;

/// <summary>
/// Encapsulates the status of a <see cref="FiberTask"/> or <see cref="FiberTask{T}"/>.
/// </summary>
public enum FiberTaskStatus
{
  Pending,
  Succeeded,
  Canceled,
  Faulted
}

/// <summary>
/// A zero-allocation async task helper for tight inner loops and game logic.
/// </summary>
/// <remarks>
/// This tasks is scheduled entirely on a single thread for use in the parent game engine.
/// </remarks>
[AsyncMethodBuilder(typeof(FiberTaskBuilder))]
public readonly struct FiberTask : IDisposable
{
  public static FiberTask CompletedTask => default;

  #region Factories

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static FiberTask Create(Func<FiberTask> factory)
    => factory();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static FiberTask<T> Create<T>(Func<FiberTask<T>> factory)
    => factory();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static FiberTask<T> FromResult<T>(T value)
    => new(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static FiberTask FromException(Exception exception)
    => FromPromise(new ExceptionPromise<Void>(exception));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static FiberTask<T> FromException<T>(Exception exception)
    => FromPromise(new ExceptionPromise<T>(exception));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static FiberYieldAwaitable Yield()
    => new();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static WhenAllBuilder WhenAll(CancellationToken cancellationToken = default)
    => new(WhenAllPromise.Create(cancellationToken));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static WhenAnyBuilder WhenAny()
    => new(WhenAnyPromise.Create());

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static FiberTask WhenAll(FiberTask a, FiberTask b)
    => WhenAll().AddTask(a).AddTask(b).Begin();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static FiberTask WhenAll(FiberTask a, FiberTask b, FiberTask c)
    => WhenAll().AddTask(a).AddTask(b).AddTask(c).Begin();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static FiberTask WhenAll(FiberTask a, FiberTask b, FiberTask c, FiberTask d)
    => WhenAll().AddTask(a).AddTask(b).AddTask(c).AddTask(d).Begin();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static FiberTask WhenAny(FiberTask a, FiberTask b)
    => WhenAny().AddTask(a).AddTask(b).Begin();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static FiberTask WhenAny(FiberTask a, FiberTask b, FiberTask c)
    => WhenAny().AddTask(a).AddTask(b).AddTask(c).Begin();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static FiberTask WhenAny(FiberTask a, FiberTask b, FiberTask c, FiberTask d)
    => WhenAny().AddTask(a).AddTask(b).AddTask(c).AddTask(d).Begin();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static FiberTask Delay(TimeSpan duration, CancellationToken cancellationToken = default)
    => FromPromise(DelayPromise.Create(duration, cancellationToken));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static FiberTask WithDelay(FiberTask task, TimeSpan duration, CancellationToken cancellationToken = default)
    => WhenAny(task, Delay(duration, cancellationToken));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static FiberTask FromPromise(IPromise<Void> promise)
    => new(promise, promise.Version);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static FiberTask<T> FromPromise<T>(IPromise<T> promise)
    => new(promise, promise.Version);

  #endregion

  internal readonly IPromise<Void>? Promise;
  internal readonly short Version;

  internal FiberTask(IPromise<Void> promise, short version)
  {
    Promise = promise;
    Version = version;
  }

  public FiberTaskStatus Status
  {
    get
    {
      if (Promise != null)
      {
        return Promise.GetStatus(Version);
      }

      return FiberTaskStatus.Succeeded;
    }
  }

  [UsedImplicitly]
  public FiberTaskAwaiter GetAwaiter()
  {
    return new FiberTaskAwaiter(this);
  }

  public void Forget()
  {
    // no-op
  }

  public void Cancel()
  {
    Promise?.Cancel(Version);
  }

  public void Dispose()
  {
    Cancel();
  }

  /// <summary>
  /// Helper for building <see cref="WhenAllPromise"/>s without allocation.
  /// </summary>
  public readonly struct WhenAllBuilder
  {
    private readonly WhenAllPromise _promise;

    internal WhenAllBuilder(WhenAllPromise promise)
    {
      _promise = promise;
    }

    public WhenAllBuilder AddTask(FiberTask task)
    {
      _promise.AddTask(task);

      return this;
    }

    public FiberTask Begin()
    {
      _promise.Advance();

      return FromPromise(_promise);
    }
  }

  /// <summary>
  /// Helper for building <see cref="WhenAnyPromise"/>s without allocation.
  /// </summary>
  public readonly struct WhenAnyBuilder
  {
    private readonly WhenAnyPromise _promise;

    internal WhenAnyBuilder(WhenAnyPromise promise)
    {
      _promise = promise;
    }

    public WhenAnyBuilder AddTask(FiberTask task)
    {
      _promise.AddTask(task);

      return this;
    }

    public FiberTask Begin()
    {
      _promise.Advance();

      return FromPromise(_promise);
    }
  }
}

/// <summary>
/// A strongly-typed result-carrying variant of <see cref="FiberTask"/>.
/// </summary>
/// <remarks>
/// This tasks is scheduled entirely on a single thread for use in the parent game engine.
/// </remarks>
[AsyncMethodBuilder(typeof(FiberTaskBuilder<>))]
public readonly struct FiberTask<T> : IDisposable
{
  internal readonly IPromise<T>? Promise;
  internal readonly short Version;
  internal readonly T? Result;

  internal FiberTask(T result)
  {
    Promise = default;
    Version = default;
    Result = result;
  }

  internal FiberTask(IPromise<T> promise, short version)
  {
    Promise = promise;
    Version = version;
    Result = default;
  }

  public FiberTaskStatus Status
  {
    get
    {
      if (Promise != null)
      {
        return Promise.GetStatus(Version);
      }

      return FiberTaskStatus.Succeeded;
    }
  }

  [UsedImplicitly]
  public FiberTaskAwaiter<T> GetAwaiter()
  {
    return new FiberTaskAwaiter<T>(this);
  }

  public void Cancel()
  {
    Promise?.Cancel(Version);
  }

  public void Dispose()
  {
    Cancel();
  }
}