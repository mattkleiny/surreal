namespace Surreal.Collections;

public class StructListTests
{
  [Test]
  public void it_should_save_items()
  {
    var list = new StructList<TestStruct> { new(0), new(1), new(2), new(3), new(4), new(5) };

    list.Count.Should().Be(6);
  }

  [Test]
  public void it_should_iterate_items()
  {
    var list = new StructList<TestStruct> { new(0), new(1), new(2), new(3), new(4), new(5) };

    foreach (ref var item in list)
    {
      item.Value.Should().BeLessThan(6);
    }
  }

  private readonly record struct TestStruct(int Value);
}