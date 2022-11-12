namespace Surreal.Collections;

public class BoundedStackTests
{
  [Test]
  public void it_should_not_push_past_bound()
  {
    var queue = new BoundedStack<int>(maxCapacity: 2);

    queue.TryPush(1).Should().BeTrue();
    queue.TryPush(2).Should().BeTrue();
    queue.TryPush(3).Should().BeFalse();
  }
}



