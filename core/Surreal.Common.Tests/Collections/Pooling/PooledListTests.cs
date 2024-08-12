namespace Surreal.Collections.Pooling;

public class PooledListTests
{
  [Test]
  public void it_should_allocate_from_shared_pool()
  {
    var list = PooledList<int>.CreateOrRent();

    list.AddRange(Enumerable.Range(0, 1000));
    list.Count.Should().Be(1000);

    list.Dispose();
    list.Count.Should().Be(0);
  }
}
