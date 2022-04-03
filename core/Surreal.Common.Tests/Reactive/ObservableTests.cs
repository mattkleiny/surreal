namespace Surreal.Reactive;

public class ObservableTests
{
  [Test]
  public void it_should_notify_on_next()
  {
    var observable = new Observable<int>();
    var sequence   = new List<int>();

    using var _ = observable.Subscribe(value => sequence.Add(value));

    observable.NotifyNext(4);
    observable.NotifyNext(2);
    observable.NotifyNext(3);
    observable.NotifyNext(1);

    Assert.That(sequence, Is.EquivalentTo(new[] { 4, 3, 2, 1 }));
  }

  [Test]
  public void it_should_notify_on_error()
  {
    var observable = new Observable<int>();
    var hasError   = false;

    using var _ = observable.Subscribe(onError: _ => hasError = true);

    observable.NotifyError(new InvalidOperationException());

    Assert.IsTrue(hasError);
  }

  [Test]
  public void it_should_notify_on_complete()
  {
    var observable = new Observable<int>();
    var isComplete = false;

    using var _ = observable.Subscribe(onCompleted: () => isComplete = true);

    observable.NotifyCompleted();

    Assert.IsTrue(isComplete);
  }
}
