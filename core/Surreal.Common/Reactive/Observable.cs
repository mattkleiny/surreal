namespace Surreal.Reactive;

/// <summary>A simple <see cref="IObservable{T}"/> implementation.</summary>
public sealed class Observable<T> : IObservable<T>
{
  private readonly LinkedList<IObserver<T>> observers = new();

  public IDisposable Subscribe(IObserver<T> observer)
  {
    observers.AddLast(observer);

    return new SubscriptionToken(this, observer);
  }

  public void Unsubscribe(IObserver<T> observer)
  {
    observers.Remove(observer);
  }

  public void NotifyNext(T value)
  {
    foreach (var observer in observers)
    {
      observer.OnNext(value);
    }
  }

  public void NotifyError(Exception exception)
  {
    foreach (var observer in observers)
    {
      observer.OnError(exception);
    }
  }

  public void NotifyCompleted()
  {
    foreach (var observer in observers)
    {
      observer.OnCompleted();
    }
  }

  /// <summary>A <see cref="IDisposable"/> which will unsubscribe an observer upon disposal.</summary>
  private sealed class SubscriptionToken : IDisposable
  {
    private readonly Observable<T> observable;
    private readonly IObserver<T>  observer;

    public SubscriptionToken(Observable<T> observable, IObserver<T> observer)
    {
      this.observable = observable;
      this.observer   = observer;
    }

    public void Dispose()
    {
      observable.Unsubscribe(observer);
    }
  }
}
