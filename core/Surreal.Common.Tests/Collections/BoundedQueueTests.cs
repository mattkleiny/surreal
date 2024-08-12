using Surreal.Collections.Bounded;

namespace Surreal.Collections;

public class BoundedQueueTests
{
  [Test]
  public void it_should_not_enqueue_past_bound()
  {
    var queue = new BoundedQueue<int>(maxCapacity: 2);

    queue.TryEnqueue(1).Should().BeTrue();
    queue.TryEnqueue(2).Should().BeTrue();
    queue.TryEnqueue(3).Should().BeFalse();
  }
}
