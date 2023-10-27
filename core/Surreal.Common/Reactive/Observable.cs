namespace Surreal.Reactive;

/// <summary>
/// A simple and direct <see cref="IObservable{T}"/> implementation.
/// </summary>
public sealed class Observable<T> : IObservable<T>
{
  private readonly List<IObserver<T>> _observers = new();

  /// <inheritdoc/>
  public IDisposable Subscribe(IObserver<T> observer)
  {
    _observers.Add(observer);

    return Disposables.Anonymous(() => { _observers.Remove(observer); });
  }

  /// <summary>
  /// Notifies all <see cref="IObserver{T}"/>s that a new item is available.
  /// </summary>
  public void NotifyNext(T value)
  {
    for (var i = _observers.Count - 1; i >= 0; i--)
    {
      _observers[i].OnNext(value);
    }
  }

  /// <summary>
  /// Notifies all <see cref="IObserver{T}"/>s that an error has occurred.
  /// </summary>
  public void NotifyError(Exception exception)
  {
    for (var i = _observers.Count - 1; i >= 0; i--)
    {
      _observers[i].OnError(exception);
    }
  }

  /// <summary>
  /// Notifies all <see cref="IObserver{T}"/>s that all items have completed sending..
  /// </summary>
  public void NotifyCompleted()
  {
    for (var i = _observers.Count - 1; i >= 0; i--)
    {
      _observers[i].OnCompleted();
    }
  }
}
