namespace Surreal.Collections;

public class PoolTests
{
  [Test]
  public void it_should_create_and_return_lists_to_shared_pool()
  {
    using var list = PooledList<int>.CreateOrRent();

    list.Add(1);
    list.Add(2);
    list.Add(3);

    Assert.That(list, Is.EqualTo(new[] { 1, 2, 3 }));
  }
}
