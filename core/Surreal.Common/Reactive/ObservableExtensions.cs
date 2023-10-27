namespace Surreal.Reactive;

/// <summary>
/// Static extensions for working with <see cref="IObservable{T}"/>s.
/// </summary>
public static class ObservableExtensions
{
  /// <summary>
  /// Subscribes the given anonymous delegates to the observable.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onNext, Action<Exception>? onError = null, Action? onCompleted = null)
    => observable.Subscribe(new AnonymousObserver<T>(onNext, onError, onCompleted));

  /// <summary>
  /// An <see cref="IObservable{T}"/> that filters values based on a predicate.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IObservable<T> Where<T>(this IObservable<T> observable, Predicate<T> predicate)
    => new WhereObservable<T>(observable, predicate);

  /// <summary>
  /// An <see cref="IObservable{T}"/> that transforms values based on a callback.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IObservable<TResult> Select<T, TResult>(this IObservable<T> observable, Func<T, TResult> transformer)
    => new SelectObservable<T, TResult>(observable, transformer);

  /// <summary>
  /// A <see cref="IObserver{T}"/> implementation based on delegate callbacks.
  /// </summary>
  private sealed class AnonymousObserver<T>(Action<T> onNext, Action<Exception>? onError, Action? onCompleted) : IObserver<T>
  {
    /// <inheritdoc/>
    public void OnNext(T value) => onNext.Invoke(value);

    /// <inheritdoc/>
    public void OnError(Exception error) => onError?.Invoke(error);

    /// <inheritdoc/>
    public void OnCompleted() => onCompleted?.Invoke();
  }

  /// <summary>
  /// A <see cref="IObservable{T}"/> that transforms values based on a 'Where' style predicate.
  /// </summary>
  private sealed class WhereObservable<T>(IObservable<T> observable, Predicate<T> predicate) : IObservable<T>
  {
    public IDisposable Subscribe(IObserver<T> observer)
    {
      return observable.Subscribe(new WhereObserver(observer, predicate));
    }

    private sealed class WhereObserver(IObserver<T> observer, Predicate<T> predicate) : IObserver<T>
    {
      public void OnNext(T value)
      {
        if (predicate(value))
        {
          observer.OnNext(value);
        }
      }

      public void OnError(Exception error)
      {
        observer.OnError(error);
      }

      public void OnCompleted()
      {
        observer.OnCompleted();
      }
    }
  }

  /// <summary>
  /// A <see cref="IObservable{T}"/> that transforms values based on a 'Select' style callback.
  /// </summary>
  private sealed class SelectObservable<T, TResult>(IObservable<T> observable, Func<T, TResult> transformer) : IObservable<TResult>
  {
    public IDisposable Subscribe(IObserver<TResult> observer)
    {
      return observable.Subscribe(new SelectObserver(observer, transformer));
    }

    private sealed class SelectObserver(IObserver<TResult> observer, Func<T, TResult> transformer) : IObserver<T>
    {
      public void OnNext(T value)
      {
        observer.OnNext(transformer(value));
      }

      public void OnError(Exception error)
      {
        observer.OnError(error);
      }

      public void OnCompleted()
      {
        observer.OnCompleted();
      }
    }
  }
}
