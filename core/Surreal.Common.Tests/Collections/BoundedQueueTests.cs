using NUnit.Framework;

namespace Surreal.Collections;

public class BoundedQueueTests
{
  [Test]
  public void it_should_not_enqueue_past_bound()
  {
    var queue = new BoundedQueue<int>(maxCapacity: 2);

    Assert.IsTrue(queue.TryEnqueue(1));
    Assert.IsTrue(queue.TryEnqueue(2));
    Assert.IsFalse(queue.TryEnqueue(3));
  }
}
