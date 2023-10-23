namespace Surreal.Common.Tests;

public class DisposablesTests
{
  [Test]
  public void it_should_create_a_delegate_based_disposable()
  {
    var wasInvoked = false;
    var disposable = Disposables.Anonymous(() => wasInvoked = true);

    disposable.Dispose();

    wasInvoked.Should().BeTrue();
  }
}
