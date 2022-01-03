using NUnit.Framework;

namespace Surreal.Collections;

public class BoundedStackTests
{
  [Test]
  public void it_should_not_push_past_bound()
  {
    var queue = new BoundedStack<int>(maxCapacity: 2);

    Assert.IsTrue(queue.TryPush(1));
    Assert.IsTrue(queue.TryPush(2));
    Assert.IsFalse(queue.TryPush(3));
  }
}
