using System.Runtime.CompilerServices;

namespace Surreal.Reactive;

/// <summary>Extension methods for working with <see cref="IObservable{T}"/>s</summary>
public static class ObservableExtensions
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IDisposable Subscribe<T>(
    this IObservable<T> observable,
    Action<T>? onNext = default,
    Action<Exception>? onError = default,
    Action? onCompleted = default
  )
  {
    return observable.Subscribe(new AnonymousObserver<T>
    {
      NextDelegate      = onNext,
      ErrorDelegate     = onError,
      CompletedDelegate = onCompleted,
    });
  }

  /// <summary>An anonymous, delegate-based <see cref="IObserver{T}"/> implementation.</summary>
  private sealed class AnonymousObserver<T> : IObserver<T>
  {
    public Action<T>?         NextDelegate      { get; set; }
    public Action<Exception>? ErrorDelegate     { get; set; }
    public Action?            CompletedDelegate { get; set; }

    public void OnNext(T value)
    {
      NextDelegate?.Invoke(value);
    }

    public void OnError(Exception error)
    {
      ErrorDelegate?.Invoke(error);
    }

    public void OnCompleted()
    {
      CompletedDelegate?.Invoke();
    }
  }
}
